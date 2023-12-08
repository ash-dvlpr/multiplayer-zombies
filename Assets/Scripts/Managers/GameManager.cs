using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public enum GameState : int {
        None    = -1,
        Startup =  0,
        InMenus =  1,
        InGame  =  2,
    }

    // ====================== Unity Code ======================
    void Awake() {
        // Mantain a single Instance
        if (Instance != null && Instance != this) DestroyImmediate(this);
        else {
            DontDestroyOnLoad(this);
            Instance = this;
        }
    }

    void Start() {
        MenuManager.OpenMenu(MenuID.MAIN, MenuID.NONE);
    }
}
