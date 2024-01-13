using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : AMenu {
    public override MenuID MenuKey { get => MenuID.PlayerUI; }

    // ==================== Configuration ====================
    [SerializeField] AudioClip gameTheme;

    // ====================== References =====================
    [field: SerializeField] public TMP_Text RoundDisplay { get; private set; }
    [field: SerializeField] public TMP_Text RemainingEnemyCountDisplay { get; private set; }
    [field: SerializeField] public TMP_Text TotalEnemyCountDisplay { get; private set; }
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
        RegisterEvents();
        base.LockCursor();
        base.OpenMenu();

        // Update UI elements
        AudioManager.ResumeAudio();
        AudioManager.PlayClip(gameTheme, false);

        HPBar.Refresh();
        OnRoundChange(NetGameManager.Instance.Round);
        OnAliveEnemiesChange(NetGameManager.Instance.AliveEnemies);
        OnTotalEnemiesChange(NetGameManager.Instance.TotalEnemies);
    }
    public override void CloseMenu() {
        DeregisterEvents();
        base.UnlockCursor();
        base.CloseMenu();
    }

    private void RegisterEvents() {
        NetGameManager.Instance.OnRoundChange += OnRoundChange;
        NetGameManager.Instance.OnAliveEnemiesChange += OnAliveEnemiesChange;
        NetGameManager.Instance.OnTotalEnemiesChange += OnTotalEnemiesChange;
    }
    private void DeregisterEvents() {
        NetGameManager.Instance.OnRoundChange -= OnRoundChange;
        NetGameManager.Instance.OnAliveEnemiesChange -= OnAliveEnemiesChange;
        NetGameManager.Instance.OnTotalEnemiesChange -= OnTotalEnemiesChange;
    }

    private void OnRoundChange(int newValue) {
        RoundDisplay?.SetText($"{newValue}");;
    }
    private void OnAliveEnemiesChange(int newValue) {
        RemainingEnemyCountDisplay?.SetText($"{newValue}");;
    }
    private void OnTotalEnemiesChange(int newValue) {
        TotalEnemyCountDisplay?.SetText($"{newValue}");;
    }

    // ===================== UI Actions ======================

}
