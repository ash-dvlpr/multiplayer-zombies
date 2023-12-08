using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : Menu {
    public override MenuID MenuKey { get => MenuID.Settings; }

    // ===================== Custom Code =====================
    public override void OpenMenu() { 
        base.OpenMenu();
    }
    public override void CloseMenu() { 
        base.CloseMenu();

        // Handle Going back to the main menu or closing the menu depending 
    }

    // ===================== UI Actions ======================
    public void OnClick_Back() {
        // Return to Main Menu
        if (GameManager.GameState.MainMenu == GameManager.CurrentState) {
            MenuManager.OpenMenu(MenuID.Main);
        }
        // Return to Pause Menu
        else { 
            MenuManager.OpenMenu(MenuID.Pause);
        }
    }
}
