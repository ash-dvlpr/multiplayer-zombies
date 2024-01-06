using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : AMenu {
    public override MenuID MenuKey { get => MenuID.PlayerUI; }

    // ====================== References =====================
    [field: SerializeField] public TMP_Text RoundDisplay { get; private set; }
    [field: SerializeField] public ResourceBar HPBar { get; private set; }
    [field: SerializeField] public ResourceBar AmmoBar { get; private set; }
    
    [SerializeField] GameObject onScreenControls;

#if UNITY_ANDROID || UNITY_IOS
    private void Awake() {
        UpdateOnScreenControlls();
    }

    public void UpdateOnScreenControlls() {
        onScreenControls.SetActive(SettingsManager.OnScreenController);
    }
#endif

    // ===================== Custom Code =====================

    public override void OpenMenu() {
        base.LockCursor();
        base.OpenMenu();

        HPBar.Refresh();

    }
    public override void CloseMenu() { 
        base.UnlockCursor();
        base.CloseMenu();
    }

    private void RegisterEvents() {
        EnemySpawner.Instance.OnRoundChange += OnRoundChange;
    }
    private void DeregisterEvents() { 
        EnemySpawner.Instance.OnRoundChange -= OnRoundChange;
    }

    private void OnRoundChange(int newValue) {
        RoundDisplay.text = $"{newValue}";
    }

    // ===================== UI Actions ======================
    
}
