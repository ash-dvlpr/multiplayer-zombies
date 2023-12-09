using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using FishNet.Transporting.Tugboat;

public class MultiPlayerLobby : FishNetLobby<Tugboat> {
    public override LobbyType Type { get => LobbyType.MultiPlayer; }

    // ====================== Variables ======================
    // TODO: Change to UTP + Realy
    // TODO: When hosting store lobby code and show in GUI

    // ===================== Custom Code =====================
    
    public override GameState PrepareLobby() {
        base.PrepareLobby(); // Ignore return type
        
        // TODO: Initialize UnityServices for UTP overriding ConfigTransports()
        // Show Lobby UI
        MenuManager.OpenMenu(MenuID.Lobby);

        return GameState.InLobby;
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }
}

