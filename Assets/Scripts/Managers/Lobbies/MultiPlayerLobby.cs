using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using static GameManager;
using FishNet.Transporting.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using FishNet.Managing;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using System;
using Unity.VisualScripting;

public class MultiPlayerLobby : FishNetLobby<FishyUnityTransport> {
    public override LobbyType Type { get => LobbyType.MultiPlayer; }

    // ====================== Variables ======================
    FishyUnityTransport utp;
    Allocation hostAllocation;
    JoinAllocation clientAllocation;

    public string LobbyCode { get; private set; } = "";

    // ===================== Custom Code =====================
    protected override void StartServer() {
        // TODO: Hardcoded value
        WaitForTaskRoutine(CreateUTPHostAllocation(4), base.StartServer);
    }
    private async Task CreateUTPHostAllocation(int maxPlayers) {
        hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections: maxPlayers);

        utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));

        LobbyCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
    }

    protected override void ConnectClient() {
        // Extract the lobby code if we are joining another lobby
        Debug.Log($"Lobby Type: {clientType}");
        if (ClientType.Client == clientType) {
            var code = ( (LobbyMenu) MenuManager.Get(MenuID.Lobby) ).LobbyCodeFieldText;
            // If a code was supplied
            if (null != code.NullIfEmpty()) { 
                LobbyCode = code;
            }
        }

        WaitForTaskRoutine(CreateUTPCientAllocation(LobbyCode), base.ConnectClient);
    }
    private async Task CreateUTPCientAllocation(string lobbyCode) {
        // Clients make a new allocation, hosts use their own host allocation
        if (ClientType.Host == clientType) {
            utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));
        }
        else if (ClientType.Client == clientType) { 
            clientAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: lobbyCode);
            utp.SetRelayServerData(new RelayServerData(clientAllocation, "dtls"));
        }
    }

    // ================== Outside Facing API ==================
    public override GameState PrepareLobby() {
        base.PrepareLobby(); // Ignore return type
        // Store UTP reference
        utp = (FishyUnityTransport) serverTransport;

        // Show Lobby UI
        MenuManager.OpenMenu(MenuID.Lobby);

        return GameState.InLobby;
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }
}

