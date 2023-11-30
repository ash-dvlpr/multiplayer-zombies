using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }
    private @PlayerControls controls;

    // ====================== Unity Code ======================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        controls = new PlayerControls();
    }

    void OnEnable() {
        controls.Enable();
    }

    void OnDisable() {
        controls.Disable();
    }

    // ================== Outside Facing API ==================
    public static Vector2 InGameMovement => Instance?.controls.InGame.Movement.ReadValue<Vector2>() ?? Vector2.zero;
    public static Vector2 InGameLookDelta => Instance?.controls.InGame.LookDelta.ReadValue<Vector2>() ?? Vector2.zero;
    public static bool InGameRunPressed => Instance?.controls.InGame.RunModifier.IsPressed() ?? false;
    public static bool InGameJumpPressed => Instance?.controls.InGame.Jump.IsPressed() ?? false;
}
