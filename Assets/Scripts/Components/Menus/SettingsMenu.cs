using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Settings; }
    
    [SerializeField] Setting onScreenControllerOption;

    Toggle _toggle;

    // ===================== Custom Code =====================
    public override void OpenMenu() {
        GameManager.ClientInMenu = true;
        PauseGame();

        base.OpenMenu();
    }
    public override void CloseMenu() {
        GameManager.ClientInMenu = false;
        UnpauseGame();

        base.CloseMenu();
    }

    // ====================== Unity Code ======================
    void Awake() {
#if UNITY_ANDROID || UNITY_IOS
        onScreenControllerOption.gameObject.SetActive(true);
        _toggle = onScreenControllerOption.GetComponentInChildren<Toggle>();
        _toggle.isOn = SettingsManager.OnScreenController;
#else
        onScreenControllerOption.gameObject.SetActive(false);
#endif
    }

    // ===================== UI Actions ======================

    //? Settings actions
    public void OnClick_ControllerSupport() {
#if UNITY_ANDROID || UNITY_IOS
        SettingsManager.OnScreenController = _toggle.isOn;
        ((PlayerUI)MenuManager.Get(MenuID.PlayerUI)).UpdateOnScreenControlls();
#endif
    }


    //? Normal actions
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
