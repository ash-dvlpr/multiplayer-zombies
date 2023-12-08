using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    // ====================== Variables ======================
    private GameState state = GameState.None;
    private GameType  type  = GameType.None;
    public enum GameState : int {
        None     = -1,
        Startup  =  0,
        MainMenu =  1,
        InGame   =  2,
        GameOver =  3,
    }
    public enum GameType : int { 
        None         = 0,
        SinglePlayer = 1, 
        MultiPlayer  = 2,
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
        TryChangeGameState(GameState.Startup);
    }

    // ===================== Game States ======================
    private void TryChangeGameState(GameState newState, GameType type = GameType.None) {
        if (state == newState) return;

        // Finite State Machine to handle possible state changes
        state = newState switch {
            GameState.Startup  => Handle_Startup(),
            GameState.MainMenu => Handle_MainMenu(),
            GameState.InGame   => Handle_InGame(type),
            _                  => Handle_Unimplemented(newState),
        };
    }

    private GameState Handle_Unimplemented(GameState newState) {
        throw new NotImplementedException($"GameManager.TryChangeGameState(): Transition to state '{newState}' has not yet been implemented.");
    }

    private GameState Handle_Startup() {
        // Initialise services
        MenuManager.Init();

        // TODO: Do other initialisation
        return Handle_MainMenu();
    }

    private GameState Handle_MainMenu() {
        MenuManager.OpenMenu(MenuID.Main);

        return GameState.MainMenu;
    }

    private GameState Handle_InGame(GameType type) {
        // TODO: Game logic
        Debug.Log($"Loading {type} game");
        MenuManager.OpenMenu(MenuID.Pause);

        return GameState.InGame;
    }
    //private GameState Handle_GameOver() { }

    // ================== Outside Facing API ==================
    public static GameState CurrentState { get => Instance?.state ?? GameState.None; }

    public static void TryLoadSingleplayerGame() {
        Instance?.TryChangeGameState(GameState.InGame, GameType.SinglePlayer);
    }
    public static void TryLoadMultiplayerGame() {
        Instance?.TryChangeGameState(GameState.InGame, GameType.MultiPlayer);
    }

    public static void ExitToTittleScreen() {
        Instance?.TryChangeGameState(GameState.MainMenu);
    }

    public static void CloseGame() { 
        // TODO: Multiplayer port cleanup?
        Application.Quit();
    }
}
