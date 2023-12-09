using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager {
    private static object LOCK = new object();
    public static bool Initialised { get; private set; } = false;
    private static @PlayerControls _controls;
    private static PlayerControls Controls {
        get { 
            if (!Initialised) Init();
            return _controls;
        } 
    }

    // ===================== Custom Code =====================
    public static void Init() {
        if (!Initialised) { 
            _controls = new PlayerControls();
            _controls?.Enable();

            Initialised = true;
        }
    }

    public static void Cleanup() {
        _controls?.Disable();
        Initialised = false;
    }

    // ================== Outside Facing API ==================
    public static Vector2 InGame_Movement => Controls.InGame.Movement.ReadValue<Vector2>();
    public static Vector2 InGame_LookDelta => Controls.InGame.LookDelta.ReadValue<Vector2>();
    public static bool InGame_RunPressed => Controls.InGame.RunModifier.IsPressed();
    public static bool InGame_JumpPressed => Controls.InGame.Jump.IsPressed();

    public static event Action<InputAction.CallbackContext> InGame_OnShoot {
        add { lock(LOCK) { Controls.InGame.Shoot.performed += value; } }
        remove { lock(LOCK) { Controls.InGame.Shoot.performed -= value; }  }
    }

    public static event Action<InputAction.CallbackContext> InGame_OnPause {
        add { lock(LOCK) { Controls.InGame.Pause.performed += value; } }
        remove { lock(LOCK) { Controls.InGame.Pause.performed -= value; }  }
    }
}
