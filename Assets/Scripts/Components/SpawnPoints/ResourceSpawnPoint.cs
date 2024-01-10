using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawnPoint : SpawnPoint {
    protected override List<SpawnPoint> SpawnPointRegistry {
        get => NetGameManager.Instance?.resourceSpawnPoints;
    }
}
