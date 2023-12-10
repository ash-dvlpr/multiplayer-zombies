using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Pause; }

    // ===================== Custom Code =====================
    // TODO: if singleplayer, pause the game when opening the pause menu 
    // TODO: when pause menu open, lock user input

    public override void OpenMenu() {
        if (LobbyType.SinglePlayer == GameManager.LobbyType) Time.timeScale = 0f;

        GameManager.ClientInPauseMenu = true;
        base.OpenMenu();
    }
    public override void CloseMenu() {
        if (LobbyType.SinglePlayer == GameManager.LobbyType) Time.timeScale = 1f;

        GameManager.ClientInPauseMenu = false;
        base.CloseMenu();
    }

    // ===================== UI Actions ======================
    public void OnClick_Resume() {
        MenuManager.OpenMenu(MenuID.PlayerUI);
    }
    public void OnClick_Settings() {
        MenuManager.OpenMenu(MenuID.Settings);
    }
    public void OnClick_MainMenu() {
        MenuManager.CloseMenu();
        GameManager.ExitToTittleScreen();
    }
}
