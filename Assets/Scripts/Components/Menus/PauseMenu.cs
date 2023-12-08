using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : Menu {
    public override MenuID MenuKey { get => MenuID.Pause; }

    // ===================== Custom Code =====================
    // TODO: if singleplayer, pause the game when opening the pause menu 
    public override void OpenMenu() { 
        base.OpenMenu();
    }
    public override void CloseMenu() { 
        base.CloseMenu();
    }

    // ===================== UI Actions ======================
    public void OnClick_Resume() {
        // Lock Mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MenuManager.CloseMenu();
    }
    public void OnClick_Settings() {
        MenuManager.OpenMenu(MenuID.Settings);
    }
    public void OnClick_MainMenu() {
        MenuManager.CloseMenu();
        GameManager.ExitToTittleScreen();
    }
}
