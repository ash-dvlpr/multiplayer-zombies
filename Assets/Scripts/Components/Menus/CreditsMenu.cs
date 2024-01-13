using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Credits; }

    // ===================== Custom Code =====================
    public override void OpenMenu() {
        GameManager.ClientInMenu = true;

        base.OpenMenu();
    }
    public override void CloseMenu() {
        GameManager.ClientInMenu = false;
        base.CloseMenu();
    }

    // ===================== UI Actions ======================

    //? Normal actions
    public void OnClick_Back() {
        // Return to Main Menu
        MenuManager.OpenMenu(MenuID.Main);
    }
}
