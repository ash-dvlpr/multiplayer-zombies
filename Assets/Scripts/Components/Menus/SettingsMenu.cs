using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : AMenu {
    public override MenuID MenuKey { get => MenuID.Settings; }
    
    [SerializeField] Setting onScreenControllerOption;
    Toggle _toggle;

    [SerializeField] Slider horizontalSensitivitySlider;
    [SerializeField] Slider verticalSensitivitySlider;


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

        // Horizontal sensitivity
        horizontalSensitivitySlider.minValue = SettingsManager.MIN_LOOK_SENSITIVITY_VALUE;
        horizontalSensitivitySlider.maxValue = SettingsManager.MAX_LOOK_SENSITIVITY_VALUE;
        horizontalSensitivitySlider.normalizedValue = SettingsManager.LookSensitivityX;

        // Vertical sensitivity
        verticalSensitivitySlider.minValue = SettingsManager.MIN_LOOK_SENSITIVITY_VALUE;
        verticalSensitivitySlider.maxValue = SettingsManager.MAX_LOOK_SENSITIVITY_VALUE;
        verticalSensitivitySlider.normalizedValue = SettingsManager.LookSensitivityY;
    }

    // ===================== UI Actions ======================

    //? Settings actions
    public void OnClick_ControllerSupport() {
#if UNITY_ANDROID || UNITY_IOS
        SettingsManager.OnScreenController = _toggle.isOn;
        ((PlayerUI)MenuManager.Get(MenuID.PlayerUI)).UpdateOnScreenControlls();
#endif
    }

    public void OnValueChanged_HorizontalSensitivity() {
        SettingsManager.LookSensitivityX = horizontalSensitivitySlider.normalizedValue; 
    }
    public void OnValueChanged_VerticalSensitivity() {
        SettingsManager.LookSensitivityY = verticalSensitivitySlider.normalizedValue; 
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
