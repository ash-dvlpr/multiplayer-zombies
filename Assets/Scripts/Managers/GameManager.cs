using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public static bool ApplicationIsQuitting { get; private set; } = false;
    public static bool UnityServicesInitialised { get; private set; } = false;
    public static bool ClientInMenu = false;

    public const string SCENE_ID_MAIN      = "SC_MainScene";
    public const string SCENE_ID_GRAVEYARD = "SC_Graveyard";

    // ====================== Variables ======================
    private GameState state = GameState.None;
    private ALobby lobby;

    public enum GameState : int {
        None     = 0, // Uninitialised
        Startup  = 1, // General Initialization
        MainMenu = 2, // Main Menu interface
        InLobby  = 3, // Lobby interface
        InGame   = 4, // Player is playing the game
        GameOver = 5, // Players died, lobby may be restarted
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
    private void TryChangeGameState(GameState newState, LobbyType newLobbyType = 0, ClientType clientType = 0) {
        if (state == newState) return;

        // Finite State Machine to handle possible state changes
        state = newState switch {
            GameState.Startup => HandleTo_Startup(),
            GameState.MainMenu => HandleTo_MainMenu(),
            GameState.InLobby => HandleTo_InLobby(newLobbyType),
            GameState.InGame => HandleTo_InGame(clientType),
            GameState.GameOver => HandleTo_GameOver(),
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

        // Initialise Multiplayeer services (will fail if no internet)
        WaitForTaskCorroutine(InitUnityServices(), UpdateMultiplayerButtonState);

        // Go to the MainMenu State
        return HandleTo_MainMenu();
    }

    private GameState HandleTo_MainMenu() {
        if (null != lobby) {
            lobby.CloseLobby();
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
            case LobbyType.MultiPlayer: lobby = new MultiPlayerLobby(); break;

            default:
                throw new NotImplementedException($"GameManager.HandleTo_InLobby(): {newLobbyType} not yet implemented.");
        }

        return lobby.PrepareLobby();
    }

    private GameState HandleTo_InGame(ClientType clientType) {
        if (GameState.InLobby == state) {
            if (ClientType.None == clientType) return state;

            // If we are in the lobby state, we have to know if we are hosting or joining
            switch (clientType) {
                case ClientType.Host: lobby.HostLobby(); break;
                case ClientType.Client: lobby.JoinLobby(); break;
            }

            return lobby.StartGame();
        }
        else if (GameState.InGame == state || GameState.GameOver == state) {
            // TODO: 
            Debug.Log("GameManager: Restarting Game");
            return lobby.RestartGame();
        }

        // Just in case
        throw new NotImplementedException($"GameManager.HandleTo_InGame(): Not yet implemented for current state: '{state}'.");
    }

    private GameState HandleTo_GameOver() {
        if (GameState.InGame == state) {
            // Show GameOver UI
            MenuManager.OpenMenu(MenuID.GameOverUI);
            
            return GameState.GameOver;
        }
        else { 
            return CurrentState;
        }
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

    private event Action onGameOver;
    public event Action OnGameOver {
        add    { lock(this) { onGameOver += value; } }
        remove { lock(this) { onGameOver -= value; } }
    }
    public void NotifyGameOver() {
        if (!ApplicationIsQuitting) {
            onGameOver?.Invoke();
            TryGameOver();
        }
    }

    //private event Action onRestart;
    //public event Action OnRestart {
    //    add    { lock(this) { onRestart += value; } }
    //    remove { lock(this) { onRestart -= value; } }
    //}
    //public void NotifyRestart() {
    //    if (!ApplicationIsQuitting) {
    //        onRestart?.Invoke();
    //    }
    //}
    
    private async Task InitUnityServices() {
        try { 
            // Initialize UnityServices for the UnityTransport
            await UnityServices.InitializeAsync();

            // If not already logged, log the user in
            if (!AuthenticationService.Instance.IsSignedIn) {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            UnityServicesInitialised = true;
            Debug.Log("Finished initializing Unity Services");
        } catch (ServicesInitializationException e) {
            UnityServicesInitialised = false;
            Debug.LogError($"GameManager.InitUnityServices(): Error initializing UnityServices: {e}");
        }
    }

    private void UpdateMultiplayerButtonState() {
        var mainMenu = (MainMenu) MenuManager.Get(MenuID.Main);
        mainMenu.MultiplayerButtonState = UnityServicesInitialised;
    }


    // ================== Outside Facing API ==================
    public static GameState CurrentState { get => Instance?.state ?? GameState.None; }
    public static LobbyType LobbyType { get => Instance?.lobby?.Type ?? LobbyType.None; }
    public static ClientType LobbyClientType { get => Instance?.lobby?.CurrentClientType ?? ClientType.None; }
    public static ALobby GetLobby { get => Instance?.lobby; }
    public static bool IsPlaying {
        get => GameState.InGame == CurrentState || GameState.GameOver == CurrentState;
    }

    //? Preparing to play
    public static void CreateSinglePlayerLobby() {
        Instance?.TryChangeGameState(GameState.InLobby, newLobbyType: LobbyType.SinglePlayer);
    }
    public static void CreateMultiPlayerLobby() {
        Instance?.TryChangeGameState(GameState.InLobby, newLobbyType: LobbyType.MultiPlayer);
    }

    //? Going into play
    public static void TryHostGame() {
        Instance?.TryChangeGameState(GameState.InGame, clientType: ClientType.Host);
    }

    public static void TryJoinGame() {
        Instance?.TryChangeGameState(GameState.InGame, clientType: ClientType.Client);
    }

    public static void TryGameOver() { 
        Instance?.TryChangeGameState(GameState.GameOver);
    }

    public static void TryRestartGame() {
        if (GameState.InGame == CurrentState || GameState.GameOver == CurrentState) { 
            Instance?.TryChangeGameState(GameState.InGame);
        }
    }

    //? Exiting
    public static void ExitToTittleScreen() {
        Instance?.TryChangeGameState(GameState.MainMenu);
    }

    public static void CloseGame() {
        Application.Quit();
    }




    //? Utilities for running async tasks
    public static void GlobalStartCorroutine(IEnumerator routine) {
        if (Instance) {
            Instance.StartCoroutine(routine);
        }
    }

    public static void WaitForTaskCorroutine(Task task, Action actionAfterWait = null) {
        GlobalStartCorroutine(WaitForTask(task, actionAfterWait));
    }

    public static IEnumerator WaitForTask(Task task, Action actionAfterWait = null) {
        //var task = asyncMethod?.Invoke();
        yield return new WaitUntil(() => task.IsCompleted);
        actionAfterWait?.Invoke();
    }
}
