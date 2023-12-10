using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;


public enum LobbyType : byte {
    None         = 0,
    SinglePlayer = 1,
    MultiPlayer  = 2,
}


/// <summary>
/// Generic GameLobby abstraction that provides an easy way to generalize server/client code.
/// </summary>
public abstract class ALobby {
    public abstract LobbyType Type { get; }

    /// <summary>
    /// Used to prepare Lobby and/or show the Lobby Menu.
    /// </summary>
    /// <returns>Resulting state from preparing the lobby.</returns>
    public abstract GameState PrepareLobby();

    /// <summary>
    /// Used to open the Lobby.
    /// </summary>
    public abstract void HostLobby();

    /// <summary>
    /// Used to join the Lobby.
    /// </summary>
    public abstract void JoinLobby();

    /// <summary>
    /// Used to disconnect from the Lobby.
    /// </summary>
    //public abstract void LeaveLobby();

    /// <summary>
    /// Used to start the game once the lobby is ready.
    /// </summary
    public abstract GameState StartGame();

    /// <summary>
    /// Used to restart the game.
    /// </summary
    public abstract GameState RestartGame();

    /// <summary>
    /// Used to close the lobby. Should be called whenever we are closing the game or returning to main menu.
    /// </summary>
    public abstract void CloseLobby();
}
