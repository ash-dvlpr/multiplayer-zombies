using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMenu : Menu {
    public override MenuID MenuKey { get => MenuID.Lobby; }

    // ===================== Custom Code =====================
    public override void OpenMenu() { 
        base.OpenMenu();
    }
    public override void CloseMenu() { 
        base.CloseMenu();
    }

    // ===================== UI Actions ======================
    public void OnClick_Host() {
        GameManager.TryHostGame();
    }
    public void OnClick_Join() {
        GameManager.TryJoinGame();
    }
    public void OnClick_Back() {
        GameManager.ExitToTittleScreen();
    }
}
