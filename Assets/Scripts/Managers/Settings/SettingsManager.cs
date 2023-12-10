using System;
using System.Collections;
using UnityEngine;

public class SettingsManager : MonoBehaviour {
    public static SettingsManager Instance { get; private set; }

    // ===================== Custom Code =====================

    // ================== Outside Facing API ==================

#if UNITY_ANDROID || UNITY_IOS
    public static bool OnScreenController {
        get => PlayerPrefs.GetInt("OnScreenController", 1) == 1;
        set => PlayerPrefs.SetInt("OnScreenController", value ? 1 : 0);
    }
#endif

}