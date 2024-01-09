using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;

public class NetGameManager : NetworkBehaviour {
    public static NetGameManager Instance { get; private set; }

    // ==================== Configuration ====================
    [SerializeField] List<EnemyController> enemyPrefabs = new();
    [SerializeField] public List<Transform> spawnPoints;
    [SerializeField] float graceTime = 1f;

    // ====================== Variables ======================
    [SyncObject]
    public readonly SyncList<PlayerController> Players = new();

    [SyncObject]
    public readonly SyncList<EnemyController> Enemies = new();
    
    public int Round { get => _round; }
    [SyncVar(OnChange = nameof(Round_OnChange))] int _round;
    bool _roundTriggered;

    private event Action<int> onRoundChange;
    public event Action<int> OnRoundChange {
        add    { lock(this) { onRoundChange += value; } }
        remove { lock(this) { onRoundChange -= value; } }
    }

    // Events used to syncronize state across server and clients (because GameManager can't be a NetworkBehaviour)
    // NOTE: ObserversRpc (remote method call on all clients)
    // NOTE: "=>": https://stackoverflow.com/questions/39453610/what-does-operator-pointing-from-field-or-a-method-mean-c-sharp

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
        // Increase the round counter, then spawn round*2 enemies
        _round++;
        CL_NotifyRoundStart();
        for (int i = 0 ; i < _round * 2 ; i++) {
            SpawnEnemy();
        }
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
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0) return;

        var position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)].gameObject;

        var spawned = Instantiate(enemyPrefab, position, Quaternion.identity);
        //Enemies.Add(spawned.GetComponent<EnemyController>());

        InstanceFinder.ServerManager.Spawn(spawned);
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
