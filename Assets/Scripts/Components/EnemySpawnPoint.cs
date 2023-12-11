using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet;
using FishNet.Object;

public class EnemySpawnPoint : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        EnemySpawner.Instance?.spawnPoints.Add(this.transform);
    }

    public override void OnStopServer() {
        base.OnStartServer();
        EnemySpawner.Instance?.spawnPoints.Remove(this.transform);
    }
}
