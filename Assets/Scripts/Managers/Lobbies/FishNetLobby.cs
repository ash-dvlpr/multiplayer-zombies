using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Managing.Server;

public enum LobbyType : byte {
    None         = 0,
    SinglePlayer = 1,
    MultiPlayer  = 2,
}

public enum ClientType : byte {
    None   = 0,
    Host   = 1,
    Client = 2,
}

public abstract class FishNetLobby<T> : Lobby where T : Transport {

    public abstract LobbyType Type { get; }
    public LocalConnectionState ServerState { get; protected set; } = LocalConnectionState.Stopped;
    public LocalConnectionState ClientState { get; protected set; } = LocalConnectionState.Stopped;

    // ====================== Variables ======================
    private NetworkManager networkManager;
    private Multipass multipass;
    private Transport serverTransport;
    protected ClientType clientType;

    // ==================== Configuration ====================
    /// <summary>
    /// Configure the NetworkManager's Transports.
    /// A propper implementation must:
    /// <list type="bullet">
    /// <item>Set the <see cref="serverTransport"/> reference.</item>
    /// <item>Use <see cref="Multipass.SetClientTransport()"/> to set multipass's client transport.</item> 
    /// </list>
    /// </summary>
    protected virtual void ConfigTransports() {
        serverTransport = multipass.GetTransport<T>();
        multipass.SetClientTransport<T>();
    }

    // ===================== Custom Code =====================
    protected virtual void StartServer() {
        serverTransport.StartConnection(true);
    }
    protected virtual void StopServer() {
        serverTransport.StopConnection(true);
    }

    protected virtual void ConnectClient() {
        networkManager.ClientManager.StartConnection();
    }
    protected virtual void DisconnectClient() {
        networkManager.ClientManager.StopConnection();
    }

    // ================== Outside Facing API ==================
    public override GameState PrepareLobby() {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        multipass = networkManager.TransportManager.GetTransport<Multipass>();
        ConfigTransports();

        return GameState.InLobby;
    }

    public override void CloseLobby() {
        DisconnectClient();
        if (ClientType.Host == clientType) StopServer();
    }

    public override void HostLobby() {
        clientType = ClientType.Host;

        GameManager.Instance.LoadCityScene();
        StartServer();
    }

    public override void JoinLobby() {
        ConnectClient();
    }

    public override GameState StartGame() {
        GameManager.Instance.NotifyRoundStart();
        MenuManager.CloseMenu();
        return GameState.InGame;
    }
}

