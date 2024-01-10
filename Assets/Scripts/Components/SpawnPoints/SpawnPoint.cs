using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet;
using FishNet.Object;

public abstract class SpawnPoint : NetworkBehaviour {
    protected abstract List<SpawnPoint> SpawnPointRegistry { get; }

    public override void OnStartServer() {
        base.OnStartServer();

        SpawnPointRegistry.Add(this);
    }

    public override void OnStopServer() {
        base.OnStartServer();
        SpawnPointRegistry.Remove(this);
    }
}