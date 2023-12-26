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

public class MultiPlayerLobby : FishNetLobby<FishyUnityTransport> {
    public override LobbyType Type { get => LobbyType.MultiPlayer; }

    // ====================== Variables ======================
    FishyUnityTransport utp;
    Allocation hostAllocation;
    JoinAllocation clientAllocation;

    bool utpInit = false;

    public string lobbyCode { get; private set; } = "";
    // TODO: When hosting store lobby code and show in GUI

    // ===================== Custom Code =====================
    protected override void StartServer() {
        // TODO: Hardcoded value
        WaitForTastRoutine(CreateUTPHostAllocation(4), base.StartServer);
    }
    private async Task CreateUTPHostAllocation(int maxPlayers) {
        hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections: maxPlayers);

        utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));

        lobbyCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
    }

    protected override void ConnectClient() {
        // TODO: clients get join code from text field
        // if (_roomIdField.text.Trim() != "") {
        // }
        WaitForTastRoutine(CreateUTPCientAllocation(lobbyCode), base.ConnectClient);
    }
    private async Task CreateUTPCientAllocation(string lobbyCode = null) {
        // Clients make a new allocation, hosts use their own host allocation
        if (ClientType.Host == clientType) {
            utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));
        }
        else { 
            clientAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: lobbyCode);
            utp.SetRelayServerData(new RelayServerData(clientAllocation, "dtls"));
        }
    }

    // ================== Outside Facing API ==================
    public override GameState PrepareLobby() {
        base.PrepareLobby(); // Ignore return type
        // Store UTP reference
        utp = (FishyUnityTransport) serverTransport;

        // TODO: Enable buttons if unity services initialized
        WaitForTastRoutine(InitUnityServices());

        // Show Lobby UI
        MenuManager.OpenMenu(MenuID.Lobby);

        return GameState.InLobby;
    }

    private async Task InitUnityServices() {
        // Initialize UnityServices for the UnityTransport
        await UnityServices.InitializeAsync();

        // If not already logged, log the user in
        if (!AuthenticationService.Instance.IsSignedIn) {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        utpInit = true;
        Debug.Log("Finished initializing Unity Services");
    }

    public override GameState RestartGame() {
        throw new System.NotImplementedException();
    }

    private void WaitForTastRoutine(Task task, Action actionAfterWait = null) {
        GlobalStartCorroutine(WaitForTask(task, actionAfterWait));
    }

    private IEnumerator WaitForTask(Task task, Action actionAfterWait = null) {
        //var task = asyncMethod?.Invoke();
        yield return new WaitUntil(() => task.IsCompleted);
        Debug.Log("[][][][][][] task completed");
        actionAfterWait?.Invoke();
    }
}

