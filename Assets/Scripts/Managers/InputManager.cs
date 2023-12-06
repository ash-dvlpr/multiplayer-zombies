using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public static Vector2 InGame_Movement => Instance?.controls.InGame.Movement.ReadValue<Vector2>() ?? Vector2.zero;
    public static Vector2 InGame_LookDelta => Instance?.controls.InGame.LookDelta.ReadValue<Vector2>() ?? Vector2.zero;
    public static bool InGame_RunPressed => Instance?.controls.InGame.RunModifier.IsPressed() ?? false;
    public static bool InGame_JumpPressed => Instance?.controls.InGame.Jump.IsPressed() ?? false;

    public static event Action<InputAction.CallbackContext> InGame_OnShoot {
        add { if (Instance) Instance.controls.InGame.Shoot.performed += value; }
        remove { if (Instance) Instance.controls.InGame.Shoot.performed -= value; }
    }

}
