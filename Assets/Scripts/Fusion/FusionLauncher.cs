using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using Fusion.Photon.Realtime;
using System.Collections.Generic;
using System;
using TMPro;
using System.Collections;
using PlayFab;



public class FusionLauncher : NetworkBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner runners;
    private NetworkRunner runnerInstance;
    public LobbyManager lm;


    public Button startButton;
    public GameObject waitingTxt;
    public Transform playerListContainer, chatContainer;
    public GameObject playerEntryPrefab, authLoading, multiplayerPanel, countdownTimerPrefab;

    
    private Dictionary<PlayerRef, PlayerEntry> playerEntries = new Dictionary<PlayerRef, PlayerEntry>();
    private static Dictionary<PlayerRef, string> PlayerNames = new();



    private const int maxPlayers = 2;

    public TMP_InputField chatIF;
    public GameObject chatPanel;

    public  ScrollRect scrollRect;

    public PlayerDataManager playerDataManager;

   public int[] levelPaths = new int[5];
    int mapSelect;

    private void Start()
    {
        levelPaths[0] = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/SinglePlayer/Multiplayer-Environment.unity");
        mapSelect = levelPaths[0];
        playerDataManager = FindAnyObjectByType<PlayerDataManager>();

    }


    public void MapSelect(int i)
    {
        if (i < levelPaths.Length)
            mapSelect = levelPaths[i];
    }


    public void Region()
    {
        //PhotonAppSettings.AppSettings.FixedRegion = "us";
        
    }

    public async void StartGame(AuthenticationValues auth)
    {


        runnerInstance =  Instantiate(runners);
       // runnerInstance = runners;
        runnerInstance.ProvideInput = true;
        runnerInstance.gameObject.SetActive(true);

        var sceneRef = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        NetworkSceneInfo info = new NetworkSceneInfo();
        info.AddSceneRef(sceneRef, LoadSceneMode.Single);

        // Add callbacks before StartGame (just in case)
        runnerInstance.AddCallbacks(this);

        var appSettings = BuildCustomAppSetting("us");


        // Await the result of StartGame
        var result = await runnerInstance.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            //GameMode = GameMode.Shared,
            SessionName = "RaceRoom",
            Scene = sceneRef,
            SceneManager = runnerInstance.GetComponent<NetworkSceneManagerDefault>(),
            AuthValues = auth,
            CustomPhotonAppSettings = appSettings,
           

        });

        if (result.Ok)
        {
            Debug.Log(" Game started successfully!");
            lm.go = true;

            // Delay RPC call until you're fully connected
            // RPC_SendPlayerName(runnerInstance.LocalPlayer, PlayerPrefs.GetString("UserName"));
        }
        else
        {
            Debug.LogWarning($" StartGame failed. Reason: {result.ShutdownReason}");

            // You can now show a UI popup or retry button
            // Example:
            // ShowErrorPopup("Failed to join or create a session. Try again later.");
        }
    }



    private FusionAppSettings BuildCustomAppSetting(string region, string customAppID = null, string appVersion = "1.0.0")
    {

        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy(); ;

        appSettings.UseNameServer = true;
        appSettings.AppVersion = appVersion;

        if (string.IsNullOrEmpty(customAppID) == false)
        {
            appSettings.AppIdFusion = customAppID;
        }

        if (string.IsNullOrEmpty(region) == false)
        {
            appSettings.FixedRegion = region.ToLower();
        }

        // If the Region is set to China (CN),
        // the Name Server will be automatically changed to the right one
        // appSettings.Server = "ns.photonengine.cn";

        return appSettings;
    }




    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SendPlayerName(PlayerRef player, string name)
    {
        Debug.Log($"[RPC] Player name set: {player} = {name}");

        // Store the name in a shared dictionary
        if (PlayerNames.ContainsKey(player))
            PlayerNames[player] = name;
        else
            PlayerNames.Add(player, name);

        // If the player's UI entry exists, set the name
        if (playerEntries.ContainsKey(player))
        {
            playerEntries[player].SetName(name);
        }
    }

    void test()
    {
        string msg = PlayerPrefs.GetString("UserName") + ": " + chatIF.text;
        GameObject obj = Instantiate(chatPanel, chatContainer);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
        StartCoroutine(ScrollToBottomNextFrame());
    }
  
    public void SendChat()
    {
        if (!string.IsNullOrWhiteSpace(chatIF.text))
        {
            string msg = PlayerPrefs.GetString("UserName") + ": " + chatIF.text;
            RPC_ChatUpdate(msg);
            chatIF.text = ""; // clear locally after sending
        }
        
        
    }



    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ChatUpdate(string msg)
    {
        if (chatPanel != null && chatContainer != null)
        {
            GameObject obj = Instantiate(chatPanel, chatContainer);
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = msg;
            StartCoroutine(ScrollToBottomNextFrame());
        }
    }

 

    public void OnStartGameClicked()
    {
        if (!runnerInstance.IsServer) return;

        Debug.Log("Host started the race!");

        // Optionally close the room
        runnerInstance.SessionInfo.IsOpen = false;
        runnerInstance.SessionInfo.IsVisible = false;
        

        int raceSceneIndex = mapSelect;
        runnerInstance.LoadScene(SceneRef.FromIndex(raceSceneIndex), new LoadSceneParameters(LoadSceneMode.Single));

        playerDataManager.MoveToScene();
        startButton.interactable = false;
    }

    public void LoadScene()
    {
        int raceSceneIndex = SceneUtility.GetBuildIndexByScenePath("Assets/Scenes/SinglePlayer/Multiplayer-Environment.unity");
        runnerInstance.LoadScene(
            SceneRef.FromIndex(raceSceneIndex),
            new LoadSceneParameters(LoadSceneMode.Additive)
        );
    }

    private void OnDisablesss()
    {
        foreach (Transform t in playerListContainer)
        {
            if (t != null)
                Destroy(t);
        }
        //playerListContainer;
    }


    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return new WaitForEndOfFrame(); // Ensure layout is rebuilt

        scrollRect.verticalNormalizedPosition = 0f; // 0 = bottom, 1 = top
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined: {player}");


        authLoading.gameObject.SetActive(false);
        multiplayerPanel.gameObject.SetActive(true);

        GameObject entryGO = Instantiate(playerEntryPrefab, playerListContainer);

        PlayerEntry entry = entryGO.GetComponent<PlayerEntry>();
        playerEntries[player] = entry;

        // Set temporary name

        entry.SetReady(true);
        entry.SetName($"Player: {player.PlayerId}");

        // Send your username if this is you (local player)
        if (player == runner.LocalPlayer)
        {
            runner.StartCoroutine(WaitForPlayerDataManagerReady(player));
        }


        runner.StartCoroutine(WaitForPlayerDataManagerReady1(player));
        // entry.SetName(PlayerDataManager.Instance.GetPlayerName(player));

        // If the name was already synced


        UpdateClientWaitingTxt();
        UpdateStartButtonState();
    }

    private IEnumerator WaitForPlayerDataManagerReady(PlayerRef player)
    {
        yield return new WaitForSeconds(1.5f);
        string username = PlayerPrefs.GetString("UserName", $"Player{player.PlayerId}");
        //PlayerDataManager.Instance.SetPlayerName(player, username);
        playerDataManager.SetPlayerName(player, username);

        

    }

    private IEnumerator WaitForPlayerDataManagerReady1(PlayerRef player )
    {
        yield return new WaitForSeconds(2f);

        playerEntries[player].SetName(playerDataManager.GetPlayerName(player));


    }

    IEnumerator SendNameAfterDelay(PlayerRef player)
    {
        yield return new WaitForSeconds(0.3f); // Wait a few frames

        if (Runner != null && Runner.LocalPlayer == player )
        {
            string userName = PlayerPrefs.GetString("UserName");
            RPC_SendPlayerName(player, userName);
        }




    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player left: {player}");

        if (playerEntries.TryGetValue(player, out var entry))
        {
            if (entry != null && entry.gameObject != null)
            {
                Destroy(entry.gameObject);
            }
            playerEntries.Remove(player);
        }
        if (runnerInstance.IsServer)
            UpdateStartButtonState();
    }

    public void Leave()
    {

        runnerInstance.Shutdown();

    }

    void UpdateClientWaitingTxt()
    {
        if(!runnerInstance.IsServer)
            waitingTxt.gameObject.SetActive(true);
    }

    private void UpdateStartButtonState()
    {
        // Only the server/host can start the game and only when all players are present
        //bool allJoined = true;
        bool allJoined = (runnerInstance.IsServer && playerEntries.Count == maxPlayers);
       // bool allJoined = (runnerInstance.IsSharedModeMasterClient && playerEntries.Count == maxPlayers);

        startButton.interactable = allJoined;
        startButton.gameObject.SetActive(allJoined);

    }

    // =======================
    // Required Callback Stubs
    // =======================

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        request.Accept(); // Accept all connections
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) 
    {
        if (runner.IsServer)
        {
         //   int i = 0;
            //foreach (var player in runner.ActivePlayers)
            //{
            //    Vector3 spawnPos = spawnPoints[i % spawnPoints.Length].position;
            //    runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, player);
            //    i++;
            //}

            // Start race countdown
           // runner.Spawn(countdownTimerPrefab); // we'll create this prefab next
        }
    }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        Debug.LogError("Shutdown Reason: " + shutdownReason);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }



}