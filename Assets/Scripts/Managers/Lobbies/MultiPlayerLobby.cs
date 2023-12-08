using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MultiPlayerLobby : GameLobby {
    public override LobbyType Type { get => LobbyType.MultiPlayer; }

    // ====================== Variables ======================

    // ===================== Custom Code =====================
    
    public override GameState PrepareLobby() {
        // TODO: Configure NetworkManager's Multipass Transport
        // TODO: Show Lobby UI
        throw new System.NotImplementedException();
        return GameState.InLobby;
    }

    public override void OpenLobby() {
        // TODO: Multipass start server
        throw new System.NotImplementedException();
    }

    public override void JoinLobby() {
        // TODO: Multipass select UTP Relay transport
        throw new System.NotImplementedException();
    }

    public override GameState StartGame() {
        // TODO: Other game loading stuff stuff
        throw new System.NotImplementedException();
        return GameState.InGame;
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }

    public override void CloseLobby() {
        throw new System.NotImplementedException();
    }
}

