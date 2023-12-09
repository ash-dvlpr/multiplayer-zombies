using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AMenu : MonoBehaviour {
    public abstract MenuID MenuKey { get; }

    // ===================== Custom Code =====================
    /// <summary>
    /// Used for showing the menu. Can be extended to do additional setup.
    /// </summary>
    public virtual void OpenMenu() { 
        // Unlock Mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Show Menu
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Used for showing the menu. Can be extended to do additional cleanup.
    /// </summary>
    public virtual void CloseMenu() {
        gameObject.SetActive(false);
    }
}
