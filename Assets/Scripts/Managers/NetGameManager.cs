using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;
using CustomExtensions.Collections;

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using Unity.VisualScripting;

public class NetGameManager : NetworkBehaviour {
    public static NetGameManager Instance { get; private set; }

    // ==================== Configuration ====================
    [Header("Enemies and Rounds")]
    [SerializeField] List<EnemyController> enemyPrefabs = new();
    [SerializeField] float graceTime = 1f;

    [field: Header("Performance")]
    [field: SerializeField] public float TimeBetweenPathRecalculations { get; private set; } = 0.5f;

    [Space(20), Header("Resources")]
    [SerializeField] List<GameObject> healthPackPrefabs = new();
    [SerializeField] List<GameObject> ammoPackPrefabs = new();

    // ====================== Variables ======================
    [Header("Variables")]
    [SyncVar(OnChange = nameof(Round_OnChange)), ShowOnly] int _round;
    [SyncVar(OnChange = nameof(TotalEnemies_OnChange)), ShowOnly] int _totalEnemies;
    [SyncVar(OnChange = nameof(AliveEnemies_OnChange)), ShowOnly] int _aliveEnemies;

    public int Round { get => _round; private set => _round = value; }
    public int TotalEnemies { get => _totalEnemies; private set => _totalEnemies = value; }
    public int AliveEnemies { get => _aliveEnemies; private set => _aliveEnemies = value; }
    
    [HideInInspector] public List<SpawnPoint> enemySpawnPoints = new();
    [HideInInspector] public List<SpawnPoint> resourceSpawnPoints = new();
    bool _roundTriggered;

    [SyncObject]
    public readonly SyncList<PlayerController> Players = new();
    [SyncObject]
    public readonly SyncList<EnemyController> Enemies = new();

    // Events used to syncronize state across server and clients (because GameManager can't be a NetworkBehaviour)
    // NOTE: ObserversRpc (remote method call on all clients)
    // NOTE: "=>": https://stackoverflow.com/questions/39453610/what-does-operator-pointing-from-field-or-a-method-mean-c-sharp

    private event Action<int> onRoundChange;
    public event Action<int> OnRoundChange {
        add    { lock(this) { onRoundChange += value; } }
        remove { lock(this) { onRoundChange -= value; } }
    }
    private event Action<int> onTotalEnemiesChange;
    public event Action<int>  OnTotalEnemiesChange {
        add    { lock(this) { onTotalEnemiesChange += value; } }
        remove { lock(this) { onTotalEnemiesChange -= value; } }
    }
    private event Action<int> onAliveEnemiesChange;
    public event Action<int>  OnAliveEnemiesChange {
        add    { lock(this) { onAliveEnemiesChange += value; } }
        remove { lock(this) { onAliveEnemiesChange -= value; } }
    }

    [ObserversRpc]
    void CL_NotifyRoundStart() => GameManager.Instance?.NotifyRoundStart();

    [ObserversRpc]
    void CL_NotifyRoundEnd() => GameManager.Instance?.NotifyRoundEnd();

    [ObserversRpc]
    void CL_NotifyGameOver() => GameManager.Instance?.NotifyGameOver();

    [ObserversRpc]
    void CL_NotifyRestart() => GameManager.Instance?.NotifyRestart();

    // ======================= NetCode ========================
    public override void OnStartServer() {
        base.OnStartServer();
        Enemies.OnChange += Enemies_OnChange;
        RestartGame();
    }

    public override void OnStopServer() {
        base.OnStopServer();
        Enemies.OnChange -= Enemies_OnChange;
        StopAllCoroutines();
    }

    public override void OnStartClient() {
        base.OnStartClient();
        MenuManager.OpenMenu(MenuID.PlayerUI);
    }

    public override void OnStopClient() {
        base.OnStartClient();
    }

    [Server]
    public void OnPlayerDied() {
        // TODO: GameOver when all players are dead

        // Get the remaining alive players
        //var players = Players.Where(
        //    p => {
        //        // Skip if no health component found
        //        if (!p.TryGet<Health>(out var health)) return false;
        //        // Skip dead players
        //        return health.IsAlive;
        //    } 
        //).ToList();

        var isGameOver = true;

        // Trigger GameOver
        if (isGameOver) {
            foreach (var player in Players) {
                player.CanControl = false;
            }
            CL_NotifyGameOver();
        }
    }

    [Server]
    public void OnPlayerNoAmmoLeft() {
        SpawnAmmoPack();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRestartGame(NetworkConnection connection = null) {
        if (connection.IsHost) {
            RestartGame(true);
        }
    }

    [Server]
    public void RestartGame(bool notify = false) {
        FadeOutClients(true);

        // Start preparing the scene and 
        StopAllCoroutines();
        _roundTriggered = false;
        Round = 0;

        DespawnAllConsumibles<ABaseCollectible>();

        Enemies.Clear();
        DespawnAllNet<EnemyController>();

        foreach (var player in Players) {
            player.Restore();
        }

        if (notify) CL_NotifyRestart();

        StartCoroutine(RoundStartDelay());
    }

    // ======================== Rounds ========================
    public void Round_OnChange(int prev, int next, bool asServer) {
        if (base.IsClient) {
            // Notify event to client side subscribers
            if (!GameManager.ApplicationIsQuitting) onRoundChange?.Invoke(next);
        }
    }
    public void AliveEnemies_OnChange(int prev, int next, bool asServer) {
        if (base.IsClient) {
            // Notify event to client side subscribers
            if (!GameManager.ApplicationIsQuitting) onAliveEnemiesChange?.Invoke(next);
        }
    }
    public void TotalEnemies_OnChange(int prev, int next, bool asServer) {
        if (base.IsClient) {
            // Notify event to client side subscribers
            if (!GameManager.ApplicationIsQuitting) onTotalEnemiesChange?.Invoke(next);
        }
    }

    [Server]
    IEnumerator RoundStartDelay() {
        yield return new WaitForSeconds(graceTime);
        FadeOutClients();
        yield return new WaitForSeconds(0.3f); // Give time for the animation to play
        if (!_roundTriggered) {
            _roundTriggered = true;
            StartRound();
        }
    }

    [Server]
    private void StartRound() {
        foreach (var player in Players) {
            // 
            player.Restore(false);
        }

        // Increase the round counter, then spawn n enemies. [n = round]
        Round++;
        TotalEnemies = Round;
        AliveEnemies = Round;
        for (int i = 0 ; i < Round ; i++) {
            SpawnEnemy();
        }

        // After the enemies have spawned, if we passed the first wave, spawn a HealthPack
        DespawnAllConsumibles<HealthPack>(); // Ensure only one medpack per round
        if (_round > 1) {
            SpawnHealthPack();
        }

        // Then notify the round start.
        CL_NotifyRoundStart();
        // Fade In
        FadeInClients();
    }

    [Server]
    private void EndRound() {
        _roundTriggered = false;
        CL_NotifyRoundEnd();
        StartCoroutine(RoundStartDelay());
    }

    // ====================== Transitions =====================
    /// <summary>
    /// Plays a fade out animation on client's screens. 
    /// </summary>
    [ObserversRpc]
    private void FadeOutClients(bool forced = false) => MenuManager.FadeOut(forced);

    [ObserversRpc(BufferLast = true)]
    private void FadeInClients(bool forced = false) => MenuManager.FadeIn(forced);


    // ======================= Entities =======================
    [Server]
    private GameObject SpawnThingNet(GameObject prefab, Vector3 position) {
        var spawned = Instantiate(prefab, position, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(spawned);
        return spawned;
    }

    [Server]
    private GameObject SpawnRandomThingNet(List<GameObject> prefabs, List<SpawnPoint> spawnPoints) {
        if (spawnPoints.Count == 0 || prefabs.Count == 0) return default;

        // This uses a custom extension method (C# feature)
        var position = spawnPoints.GetRandom().transform.position;
        var prefab = prefabs.GetRandom().gameObject;

        return SpawnThingNet(prefab, position);
    }

    [Server]
    private void DespawnAllNet<T>() where T : NetworkBehaviour { 
        var objects = from o in Resources.FindObjectsOfTypeAll<T>()
            where o.IsSpawned && !o.IsDeinitializing select o;

        foreach (var obj in objects) {
            base.Despawn(obj.gameObject);
        }
    }

    // ===================== Consumibles ======================
    [Server]
    private void SpawnHealthPack() {
        SpawnRandomThingNet(healthPackPrefabs, resourceSpawnPoints);
    }

    [Server]
    private void SpawnAmmoPack() {
        SpawnRandomThingNet(ammoPackPrefabs, resourceSpawnPoints);
    }

    [Server]
    private void DespawnAllConsumibles<C>() where C : ABaseCollectible => DespawnAllNet<C>();


    // ======================== Enemies =======================
    [Server]
    public void Enemies_OnChange(SyncListOperation op, int index, EnemyController oldItem, EnemyController newItem, bool asServer) {
        // If an enemy died/was removed
        if (SyncListOperation.RemoveAt == op) {
            var remainingEnemies = Enemies;
            AliveEnemies = Enemies.Count;

            // If there are no enemies remaining
            if (remainingEnemies.Count == 0) {
                EndRound();
            }
        }
    }

    [Server]
    void SpawnEnemy() {
        if (enemySpawnPoints.Count == 0 || enemyPrefabs.Count == 0) return;

        // This uses a custom extension method (C# feature)
        var position = enemySpawnPoints.GetRandom().transform.position;
        var enemyPrefab = enemyPrefabs.GetRandom().gameObject;

        var spawned = SpawnThingNet(enemyPrefab, position);
    }
    
    // ====================== Unity Code ======================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) {
            // Oldest instance should be kept
            Destroy(this);
        }
        else {
            Instance = this;
        }
    }
}
