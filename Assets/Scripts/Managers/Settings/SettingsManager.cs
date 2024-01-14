using System;
using System.Collections;
using UnityEngine;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager Instance { get; private set; }


    // ====================== Variables ======================
    public static int MIN_LOOK_SENSITIVITY_VALUE = 10;
    public static int MAX_LOOK_SENSITIVITY_VALUE = 200;

    public float _lookSensitivityX;
    public float _lookSensitivityY;

    // ===================== Custom Code =====================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;

            Init();
        }
    }

    void Init() {
        _lookSensitivityX = PlayerPrefs.GetFloat("LookSensitivityX", 0.6f);
        _lookSensitivityY = PlayerPrefs.GetFloat("LookSensitivityY", 0.6f);
    }

    // ================== Outside Facing API ==================

#if UNITY_ANDROID || UNITY_IOS
    public static bool OnScreenController {
        get => PlayerPrefs.GetInt("OnScreenController", 1) == 1;
        set => PlayerPrefs.SetInt("OnScreenController", value ? 1 : 0);
    }
#endif

    public static float LookSensitivityX {
        get => Instance?._lookSensitivityX ?? 0f;
        set {
            if (Instance) Instance._lookSensitivityX = value;
            PlayerPrefs.SetFloat("LookSensitivityX", value);
        }
    }
    public static float LookSensitivityY {
        get => Instance?._lookSensitivityY ?? 0f;
        set {
            if (Instance) Instance._lookSensitivityY = value;
            PlayerPrefs.SetFloat("LookSensitivityY", value);
        }
    }
}