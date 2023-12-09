using FishNet.Managing.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using FishNet.Transporting.Yak;

public class SinglePlayerLobby : FishNetLobby<Yak> {
    public override LobbyType Type { get => LobbyType.SinglePlayer; }

    // ====================== Variables ======================
    
    
    // ===================== Constructor =====================
    public SinglePlayerLobby() {
        this.clientType = ClientType.Host;
    }

    // ===================== Custom Code =====================
    public override GameState PrepareLobby() {
        base.PrepareLobby(); // Ignore return type

        HostLobby();
        return StartGame();
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }
}

