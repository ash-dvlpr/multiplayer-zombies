using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemySpawner : NetworkBehaviour {
    public static EnemySpawner Instance { get; private set; }

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

    // ======================= NetCode ========================
    public override void OnStartServer() {
        base.OnStartServer();
        Enemies.OnChange += Enemies_OnChange;
        RestartGame();
    }

    public override void OnStopServer() {
        base.OnStopServer();
        StopAllCoroutines();
    }

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

    public void Round_OnChange(int prev, int next, bool asServer) {
        // Round changed
        if (base.IsClient) { 
        
        }
    }

    [Server]
    public void RestartGame() {
        StopAllCoroutines();
        _roundTriggered = false;
        _round = 0;

        StartCoroutine(RoundStartDelay());
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

    [ObserversRpc]
    void CL_NotifyRoundStart() => GameManager.Instance?.NotifyRoundStart();

    [ObserversRpc]
    void CL_NotifyRoundEnd() => GameManager.Instance?.NotifyRoundEnd();


    [Server]
    IEnumerator RoundStartDelay() {
        yield return new WaitForSeconds(graceTime);
        if (!_roundTriggered) {
            _roundTriggered = true;
            StartRound();
        }
    }

    [Server]
    void SpawnEnemy() {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0) return;

        var position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        var enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)].gameObject;

        var spawned = Instantiate(enemyPrefab, position, Quaternion.identity);
        InstanceFinder.ServerManager.Spawn(spawned);
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
