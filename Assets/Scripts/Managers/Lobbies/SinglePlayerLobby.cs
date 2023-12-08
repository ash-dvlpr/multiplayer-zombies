using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static Resource;

public class SinglePlayerLobby : GameLobby {
    public override LobbyType Type { get => LobbyType.SinglePlayer; }

    // ====================== Variables ======================

    // ===================== Custom Code =====================
    public override void PrepareLobby() {
        return GameState.InLobby;
    }
    public override void CloseLobby() {
        throw new System.NotImplementedException();
    }

    public override void JoinLobby() {
        throw new System.NotImplementedException();
    }


    public override GameState StartGame() {
        throw new System.NotImplementedException();
    }
}

