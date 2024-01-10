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

public class NetGameManager : NetworkBehaviour {
    public static NetGameManager Instance { get; private set; }

    // ==================== Configuration ====================
    [Header("Enemies and Rounds")]
    [SerializeField] List<EnemyController> enemyPrefabs = new();
    [SerializeField] float graceTime = 1f;

    [Header("Resources")]
    [SerializeField] List<GameObject> healthPackPrefabs = new();
    [SerializeField] List<GameObject> ammoPackPrefabs = new();

    // ====================== Variables ======================
    [Header("Variables")]
    [SyncVar(OnChange = nameof(Round_OnChange)), ShowOnly] int _round;
    public int Round { get => _round; }
    
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

    [Server]
    public void OnPlayerDied() {
        // TODO: GameOver when all players are dead

        // Get the remaining alive players
        //var players = EnemySpawner.Instance?.Players.Where(
        //    p => p.PlayerHealth.IsAlive
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
        StopAllCoroutines();
        _roundTriggered = false;
        _round = 0;

        DespawnEnemies();
        foreach (var player in Players) {
            player.Restore();
        }

        if (notify) CL_NotifyRestart();
        // TODO: Transition

        StartCoroutine(RoundStartDelay());
    }

    // ======================== Rounds ========================
    public void Round_OnChange(int prev, int next, bool asServer) {
        if (base.IsClient) {
            // Notify event to client side subscribers
            if (!GameManager.ApplicationIsQuitting) onRoundChange?.Invoke(next);
        }
    }

    [Server]
    public void StartRound() {
        // Increase the round counter, then spawn n enemies. [n = round]
        _round++;
        for (int i = 0 ; i < _round ; i++) {
            SpawnEnemy();
        }

        // After the enemies have spawned, if we passed the first wave, spawn a HealthPack
        if (_round > 1) {
            SpawnHealthPack();
        }

        // Then notify the round start.
        CL_NotifyRoundStart();
    }

    [Server]
    public void EndRound() {
        _roundTriggered = false;
        CL_NotifyRoundEnd();
        StartCoroutine(RoundStartDelay());
    }

    [Server]
    IEnumerator RoundStartDelay() {
        yield return new WaitForSeconds(graceTime);
        if (!_roundTriggered) {
            _roundTriggered = true;
            StartRound();
        }
    }

    // ======================= Entities =======================
    [Server]
    private GameObject SpawnThingNet(GameObject prefab, Vector3 position) {
        var spawned = Instantiate(prefab, position, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(spawned);
        return spawned;
    }

    [Server]
    private void SpawnHealthPack() {
        if (resourceSpawnPoints.Count == 0 || healthPackPrefabs.Count == 0) return;

        // This uses a custom extension method (C# feature)
        var position = resourceSpawnPoints.GetRandom().transform.position;
        var prefab = healthPackPrefabs.GetRandom().gameObject;

        var spawned = SpawnThingNet(prefab, position);
    }

    [Server]
    private void SpawnAmmoPack() {
        if (resourceSpawnPoints.Count == 0 || ammoPackPrefabs.Count == 0) return;

        // This uses a custom extension method (C# feature)
        var position = resourceSpawnPoints.GetRandom().transform.position;
        var prefab = ammoPackPrefabs.GetRandom().gameObject;

        var spawned = SpawnThingNet(prefab, position);
    }

    // ======================== Enemies =======================
    [Server]
    public void Enemies_OnChange(SyncListOperation op, int index, EnemyController oldItem, EnemyController newItem, bool asServer) {
        // If an enemy died/was removed
        if (SyncListOperation.RemoveAt == op) {
            var remainingEnemies = Enemies;

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

    [Server]
    private void DespawnEnemies() {
        Debug.Log($"Enemies count: {Enemies.Count}");
        Enemies.Clear();

        var allEnemies = Resources.FindObjectsOfTypeAll<EnemyController>();

        foreach (var enemy in allEnemies) {
            base.Despawn(enemy.gameObject);
        }
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
