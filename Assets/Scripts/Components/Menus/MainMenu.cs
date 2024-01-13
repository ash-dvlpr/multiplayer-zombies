using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Main; }

    // ==================== Configuration ====================
    [SerializeField] AudioClip mainTheme;

    // ====================== References =====================
    [field: SerializeField] public Button MultiplayerButton { get; private set; }
    public bool MultiplayerButtonState {
        get => MultiplayerButton?.interactable ?? false;
        set {
            if (MultiplayerButton) {
                MultiplayerButton.interactable = value;
            }
        }
    }

    // ===================== Custom Code =====================
    public override void OpenMenu() {
        base.OpenMenu();
        AudioManager.PlayClip(mainTheme, false);
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
