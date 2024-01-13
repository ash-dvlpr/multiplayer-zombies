using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet;
using FishNet.Object;

public class EnemySpawnPoint : SpawnPoint {
    protected override List<SpawnPoint> SpawnPointRegistry {
        get => NetGameManager.Instance?.enemySpawnPoints;
    }
}
