using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public enum LobbyType : int {
    None         = 0,
    SinglePlayer = 1,
    MultiPlayer  = 2,
}

/// <summary>
/// Generic GameLobby abstraction that provides a 
/// </summary>
public abstract class GameLobby {
    public abstract LobbyType Type { get; }

    // ================== Outside Facing API ==================
    /// <summary>
    /// Used to prepare Lobby.
    /// </summary>
    public abstract void PrepareLobby();

    /// <summary>
    /// Used to open the Lobby.
    /// </summary>
    public abstract void OpenLobby();

    /// <summary>
    /// Used to join the Lobby.
    /// </summary>
    public abstract void JoinLobby();

    /// <summary>
    /// Used to start the game once the lobby and players are ready.
    /// </summary
    public abstract GameManager.GameState StartGame();

    /// <summary>
    /// Used to close the lobby. Should be called whenever we are closing the game or returning to main menu.
    /// </summary>
    public abstract void CloseLobby();

}

