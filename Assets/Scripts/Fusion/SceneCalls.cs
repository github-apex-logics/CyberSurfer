using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using LightDI.Examples;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SceneCalls : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Header("Spawning")]
    public NetworkObject playerPrefab;              // Must be registered in NetworkProjectConfig

    public Transform[] spawnPoints;

    [Header("Countdown")]
    public NetworkObject countdownTimerPrefab;      // Also registered in NetworkProjectConfig

    private NetworkRunner _myrunner;
    // public NetworkRunner runnerInstance;



    public FixedJoystick joystick;
    public FixedJoystick joystick2;


    public CyberNetworkController localPlayerController;


    // Start is called before the first frame update
    void Start()
    {
        _myrunner = FindAnyObjectByType<NetworkRunner>();
        _myrunner.AddCallbacks(this);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void OnSceneLoadDone(NetworkRunner runner)
    {

        _myrunner = runner;
        runner.ProvideInput = true;
        if (!runner.IsServer) return;

        Debug.Log("Scene Load Done on server. Waiting for players to be ready...");
        StartCoroutine(WaitForAllPlayersAndStartCountdown());
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
    {
        // Shut down old runner with the specific reason so your OnShutdown can branch if needed
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        // Create the new runner (e.g., from a prefab you keep around)
        var newRunner = Instantiate(_myrunner);

        // Start with HostMigration args
        var result = await newRunner.StartGame(new StartGameArgs
        {
            HostMigrationToken = token,
            HostMigrationResume = HostMigrationResume,
            // ... your other args (Scene, SessionProps, etc.)
        });

        if (!result.Ok)
            Debug.LogWarning(result.ShutdownReason);
        else
            Debug.Log("Host migration complete.");
    }

    // Step 3
    void HostMigrationResume(NetworkRunner runner)
    {
        // Recreate NOs from the last pushed snapshot
        foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())
        {
            // Default pose
            Vector3 pos = default;
            Quaternion rot = default;
            bool haveTRSP = false;

            // In Fusion 2, use NetworkTRSP (or NetworkTransform) to read spatial state
            if (resumeNO.TryGetBehaviour<NetworkTRSP>(out var trsp))
            {
                // State is a ref NetworkTRSPData containing Position/Rotation/Scale/Parent
                var data = trsp.Data;
                pos = data.Position;
                rot = data.Rotation;
                haveTRSP = true;
            }
            else if (resumeNO.TryGetBehaviour<NetworkTransform>(out var ntx))
            {
                var data = ntx.Data;
                pos = data.Position;
                rot = data.Rotation;
                haveTRSP = true;
            }

            runner.Spawn(
              resumeNO,
              position: haveTRSP ? pos : default,
              rotation: haveTRSP ? rot : default,
              onBeforeSpawned: (r, newNO) =>
              {
                  // Copy full state from snapshot object to the freshly spawned one
                  newNO.CopyStateFrom(resumeNO);

                  // Optionally copy specific behaviours only
                  //if (resumeNO.TryGetBehaviour<MyCustomNetworkBehaviour>(out var oldB))
                  //{
                  //    var newB = newNO.GetComponent<MyCustomNetworkBehaviour>();
                  //    if (newB != null) newB.CopyStateFrom(oldB);
                  //}
              }
            );
        }

    }



    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (localPlayerController == null || !localPlayerController.HasInputAuthority)
            return;
        float rotDelta = localPlayerController.accumulatedRotationDelta;
        float yawDelta = localPlayerController.targetPitch;
        // localPlayerController.accumulatedRotationDelta = 0f;

        PlayerInputData data = new PlayerInputData
        {
            rotationDelta = rotDelta,
            rotationPitchDelta = yawDelta,


        };
        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {


        if (runner.IsServer) return; // Only handle this on client

        Debug.Log("Host left the game");

        if (!runner.IsServer)
        {
            // This client was disconnected
            //SceneManager.LoadScene("MainMenu");
            NetworkedManager.Instance.winPanel.SetActive(true);
        }
        else
        {
            // The host left — show Win Panel
            //UIManager.Instance.ShowWinPanel("Opponent Disconnected");
            NetworkedManager.Instance.winPanel.SetActive(true);
        }


        //NetworkManager.Instance.LoadScene(0);
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }



    private IEnumerator WaitForAllPlayersAndStartCountdown()
    {
        while (NetworkedManager.Instance.ReadyPlayerCount < _myrunner.ActivePlayers.Count())
        {
            yield return new WaitForSeconds(0.2f);
            Debug.Log(NetworkedManager.Instance.ReadyPlayerCount + " < " + _myrunner.ActivePlayers.Count());
        }

        Debug.Log("All players ready. Spawning players...");
        SpawnAllPlayers();

        Debug.Log("Starting countdown...");
        yield return new WaitForSeconds(2f);
        NetworkObject timer = _myrunner.Spawn(countdownTimerPrefab);
    }
    private void SpawnAllPlayers()
    {
        int i = 0;
        foreach (var player in _myrunner.ActivePlayers)
        {
            Vector3 spawnPos = spawnPoints[i % spawnPoints.Length].position;
            Quaternion spawnRot = spawnPoints[i % spawnPoints.Length].rotation;
            _myrunner.Spawn(playerPrefab, spawnPos, spawnRot, inputAuthority: player);
            i++;
        }
    }



    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning("Runner shut down: " + shutdownReason);

        if (shutdownReason == ShutdownReason.DisconnectedByPluginLogic ||
         shutdownReason == ShutdownReason.GameClosed ||
         shutdownReason == ShutdownReason.ConnectionRefused  )
        {
            // Show UI or return to main menu
            //SceneManager.LoadScene("MainMenu");
            NetworkedManager.Instance.winPanel.SetActive(true);
        }

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
}
