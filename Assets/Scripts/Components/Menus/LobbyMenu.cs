using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Lobby; }
    


    [SerializeField] TMP_InputField lobbyCodeInputField;
    public string LobbyCodeFieldText { get => lobbyCodeInputField.text.Trim(); }

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
