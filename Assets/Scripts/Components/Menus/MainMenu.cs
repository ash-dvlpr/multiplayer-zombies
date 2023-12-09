using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Main; }

    // ===================== Custom Code =====================
    public override void OpenMenu() { 
        base.OpenMenu();
    }
    public override void CloseMenu() { 
        base.CloseMenu();
    }

    // ===================== UI Actions ======================
    public void OnClick_Singleplayer() {
        GameManager.CreateSinglePlayerLobby();
    }
    public void OnClick_Multiplayer() {
        GameManager.CreateMultiPlayerLobby();
    }
    public void OnClick_Settings() { 
        MenuManager.OpenMenu(MenuID.Settings);
    }
    public void OnClick_CloseGame() {
        GameManager.CloseGame();
    }
}
