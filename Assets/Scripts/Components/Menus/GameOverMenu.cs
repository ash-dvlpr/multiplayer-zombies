using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.GameOverUI; }

    // ====================== References =====================
    [field: SerializeField] public TMP_Text RoundsDisplay { get; private set; }
    [field: SerializeField] public Button RestartButton { get; private set; }


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

        // Enable the restart button if we are the host
        var buttonActive = 
            ClientType.Host == GameManager.LobbyClientType;
        RestartButton.interactable = buttonActive;   
    }

    // ===================== UI Actions ======================
    public void OnClick_Restart() {
        // If we are the host
        if (ClientType.Host == GameManager.LobbyClientType) { 
            GameManager.TryRestartGame();
        }
    }
    public void OnClick_MainMenu() {
        MenuManager.CloseMenu();
        GameManager.ExitToTittleScreen();
    }
}
