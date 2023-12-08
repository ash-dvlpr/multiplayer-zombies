using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public static bool ApplicationIsQuitting { get; private set; } = false;

    private const int SCENE_ID_MAIN = 0;
    private const int SCENE_ID_CITY = 1;

    // ====================== Variables ======================
    private GameState state = GameState.None;
    private GameLobby lobby;
    public enum GameState : int {
        None     = -1, // Uninitialised
        Startup  =  0, // General Initialization
        MainMenu =  1, // Main Menu interface
        InLobby  =  2, // Lobby interface
        InGame   =  3, // Player is playing the game
        GameOver =  4, // Players died, lobby may be restarted
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

    void OnApplicationQuit() {
        ApplicationIsQuitting = true;

        // Cleanup in reverse order
        MenuManager.Cleanup();
        InputManager.Cleanup();
    }

    // ===================== Game States ======================
    private void TryChangeGameState(GameState newState, LobbyType newLobbyType = LobbyType.None, bool isHost = false) {
        if (state == newState) return;

        // Finite State Machine to handle possible state changes
        state = newState switch {
            GameState.Startup => HandleTo_Startup(),
            GameState.MainMenu => HandleTo_MainMenu(),
            GameState.InLobby => HandleTo_InLobby(newLobbyType),
            GameState.InGame => HandleTo_InGame(isHost),
            _ => Handle_Unimplemented(newState),
        };
    }

    private GameState Handle_Unimplemented(GameState newState) {
        throw new NotImplementedException($"GameManager.TryChangeGameState(): Transition to state '{newState}' has not yet been implemented.");
    }

    private GameState HandleTo_Startup() {
        // Initialization of services
        InputManager.Init();
        MenuManager.Init();

        // Go to the MainMenu State
        return HandleTo_MainMenu();
    }

    private GameState HandleTo_MainMenu() {
        // TODO: Coming back from the game
        if (null != lobby) { 
            // TODO: Close and Clean up Lobby

            UnloadScene(SCENE_ID_CITY);
        }

        MenuManager.OpenMenu(MenuID.Main);
        return GameState.MainMenu;
    }

    private GameState HandleTo_InLobby(LobbyType newLobbyType) {
        // Do nothing if no LobbyType specified
        if (LobbyType.None == newLobbyType) return GameState.MainMenu;

        switch (newLobbyType) {
            // Create the lobby
            case LobbyType.SinglePlayer: lobby = new SinglePlayerLobby(); break;
            //case LobbyType.MultiPlayer: lobby = new MultiPlayerLobby(); break;

            default:
                throw new NotImplementedException($"GameManager.HandleTo_InLobby(): {newLobbyType} not yet implemented.");
        }

        lobby.PrepareLobby();
        return GameState.InLobby;
    }

    private GameState HandleTo_InGame(bool isHost) {
        if (GameState.InLobby == state) {
            // If we are in the lobby state, we have to know if we are hosting or joining
            if (isHost) lobby.OpenLobby();
            else lobby.JoinLobby();

            return lobby.StartGame();
        }
        else if (GameState.InGame == state || GameState.GameOver == state) {
            return lobby.RestartGame();
        }

        // Just in case
        throw new NotImplementedException($"GameManager.HandleTo_InGame(): Not yet implemented for current state: '{state}'.");
    }

    //private GameState HandleTo_GameOver() { }

    // =================== Scene Management ===================
    private static void LoadScene(int sceneBuildIndex) {
        SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Additive);
    }
    private static void UnloadScene(int sceneBuildIndex) {
        SceneManager.UnloadSceneAsync(sceneBuildIndex);
    }

    //? Exposed methods for the Lobby Code
    public void LoadCityScene() {
        LoadScene(SCENE_ID_CITY);
    }

    // ======================== Events ========================
    private event Action onRoundStart;
    public event Action OnRoundStart {
        add    { lock(this) { onRoundStart += value; } }
        remove { lock(this) { onRoundStart -= value; } }
    }
    public void NotifyRoundStart() {
        if (!ApplicationIsQuitting) {
            onRoundStart?.Invoke();
        }
    }

    private event Action onRoundEnd;
    public event Action OnRoundEnd {
        add    { lock(this) { onRoundEnd += value; } }
        remove { lock(this) { onRoundEnd -= value; } }
    }
    public void NotifyRoundEnd() {
        if (!ApplicationIsQuitting) {
            onRoundEnd?.Invoke();
        }
    }

    // ================== Outside Facing API ==================
    public static GameState CurrentState { get => Instance?.state ?? GameState.None; }

    //? Preparing to play
    public static void CreateSinglePlayerLobby() {
        Instance?.TryChangeGameState(GameState.InLobby, newLobbyType: LobbyType.SinglePlayer);
    }
    public static void CreateMultiPlayerLobby() {
        Instance?.TryChangeGameState(GameState.InLobby, newLobbyType: LobbyType.MultiPlayer);
    }

    //? Going into play
    public static void TryHostGame() {
        Instance?.TryChangeGameState(GameState.InGame, isHost: true);
    }

    public static void TryJoinGame() {
        Instance?.TryChangeGameState(GameState.InGame, isHost: false);
    }

    public static void TryStartGame() { 
        Instance?.TryChangeGameState(GameState.InGame);
    }

    //? Exiting
    public static void ExitToTittleScreen() {
        Instance?.TryChangeGameState(GameState.MainMenu);
    }

    public static void CloseGame() {
        Application.Quit();
    }
}
