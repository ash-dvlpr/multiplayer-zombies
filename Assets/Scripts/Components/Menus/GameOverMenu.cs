using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.GameOverUI; }

    // ====================== References =====================
    [field: SerializeField] public TMP_Text RoundsDisplay { get; private set; }


    // ===================== Custom Code =====================

    public override void OpenMenu() {
        base.UnlockCursor();
        base.OpenMenu();

        UpdateGUIElements();
    }
    public override void CloseMenu() {
        GameManager.ClientInMenu = false;

        base.CloseMenu();
    }

    private void UpdateGUIElements() {
        // Set round's display text
        RoundsDisplay.text = $"{EnemySpawner.Instance?.Round ?? 0}";
    }

    // ===================== UI Actions ======================
    public void OnClick_Restart() {
        // Check if HOST of lobby
        Debug.Log("TODO: Restart Game");
        //MenuManager.CloseMenu();
        //GameManager.ExitToTittleScreen();
    }
    public void OnClick_MainMenu() {
        MenuManager.CloseMenu();
        GameManager.ExitToTittleScreen();
    }
}
