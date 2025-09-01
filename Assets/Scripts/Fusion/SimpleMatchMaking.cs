using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;

public class SimpleMatchmaking : MonoBehaviour, INetworkRunnerCallbacks {
    public NetworkRunner runnerPrefab;
    public Button startButton;
    public Transform playerListContainer;
    public GameObject playerEntryPrefab;

    private NetworkRunner runner;
    private Dictionary<PlayerRef, PlayerEntry> playerEntries = new Dictionary<PlayerRef, PlayerEntry>();
    private const int maxPlayers = 4;

    async void Start() {
        // Spawn and start the NetworkRunner in Host mode
        runner = Instantiate(runnerPrefab);
        runner.ProvideInput = true;

        runner.AddCallbacks(this);

        await runner.StartGame(new StartGameArgs {
            GameMode = GameMode.Host,
            SessionName = "MyRoom",
            PlayerCount = maxPlayers,
            IsVisible = true,
            IsOpen = true,
           
        });

        startButton.interactable = false;
        startButton.onClick.AddListener(OnStartGameClicked);
    }

    private void OnStartGameClicked() {
        Debug.Log("Game started by Host!");
        // In a real project, you would now load the game scene or trigger game start logic
        runner.SessionInfo.IsOpen = false;
        runner.SessionInfo.IsVisible = false;

        // Example: just log and freeze UI for now
        startButton.interactable = false;
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"Player joined: {player}");

        GameObject entryGO = Instantiate(playerEntryPrefab, playerListContainer);
        PlayerEntry entry = entryGO.GetComponent<PlayerEntry>();
        entry.SetName($"Player {player.PlayerId}");
        entry.SetReady(false);
        playerEntries[player] = entry;

        UpdateStartButtonState();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log($"Player left: {player}");

        if (playerEntries.TryGetValue(player, out var entry)) {
            Destroy(entry.gameObject);
            playerEntries.Remove(player);
        }

        UpdateStartButtonState();
    }

    private void UpdateStartButtonState() {
        // Only the server/host can start the game and only when all players are present
        bool allJoined = runner.IsServer && playerEntries.Count == maxPlayers;
        startButton.interactable = allJoined;
    }

    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }

    // =======================
    // Required Callback Stubs

}
