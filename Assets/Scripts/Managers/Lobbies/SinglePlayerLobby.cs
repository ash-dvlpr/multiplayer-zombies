using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static Resource;

public class SinglePlayerLobby : GameLobby {
    public override LobbyType Type { get => LobbyType.SinglePlayer; }

    // ====================== Variables ======================

    // ===================== Custom Code =====================
    public override GameState PrepareLobby() {
        OpenLobby();
        JoinLobby();
        return StartGame();
    }

    public override void OpenLobby() {
        // TODO: Multipass: select Yak server
    }

    public override void JoinLobby() {
        // TODO: Multipass select Yak transport
    }

    public override GameState StartGame() {
        // TODO: Other game loading stuff stuff
        return GameState.InGame;
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }

    public override void CloseLobby() {
        throw new System.NotImplementedException();
    }
}

