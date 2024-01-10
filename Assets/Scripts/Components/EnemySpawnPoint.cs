using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet;
using FishNet.Object;

public class EnemySpawnPoint : NetworkBehaviour {
    public override void OnStartServer() {
        base.OnStartServer();

        NetGameManager.Instance?.enemySpawnPoints.Add(this.transform);
    }

    public override void OnStopServer() {
        base.OnStartServer();
        NetGameManager.Instance?.enemySpawnPoints.Remove(this.transform);
    }
}
