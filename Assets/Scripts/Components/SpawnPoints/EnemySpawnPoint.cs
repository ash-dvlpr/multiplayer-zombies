using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : SpawnPoint {
    protected override List<SpawnPoint> SpawnPointRegistry {
        get => NetGameManager.Instance?.enemySpawnPoints;
    }
}
