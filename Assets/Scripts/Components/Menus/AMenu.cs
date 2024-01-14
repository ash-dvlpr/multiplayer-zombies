using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMenu : MonoBehaviour {
    public abstract MenuID MenuKey { get; }

    // ==================== Configuration ====================
    [SerializeField] protected GameObject firstSelected;

    // ===================== Custom Code =====================
    /// <summary>
    /// Used for showing the menu. Can be extended to do additional setup.
    /// </summary>
    public virtual void OpenMenu() {
        // Show Menu
        MenuManager.ResetSelectedUIObject(firstSelected);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Used for showing the menu. Can be extended to do additional cleanup.
    /// </summary>
    public virtual void CloseMenu() {
        MenuManager.ResetSelectedUIObject();
        gameObject.SetActive(false);
    }

    protected void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    protected void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    protected void PauseGame() {
        if (LobbyType.SinglePlayer == GameManager.LobbyType) Time.timeScale = 0f;
    }

    protected void UnpauseGame() {
        if (LobbyType.SinglePlayer == GameManager.LobbyType) Time.timeScale = 1f;
    }
}
