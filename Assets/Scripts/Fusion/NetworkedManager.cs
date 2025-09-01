using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkedManager : NetworkBehaviour
{
    public static NetworkedManager Instance;

    // [Networked] public int ReadyPlayerCount { get; set; }

    private HashSet<PlayerRef> readyPlayers = new HashSet<PlayerRef>();
    private NetworkRunner _myrunner;
    public int ReadyPlayerCount = 2;

    [Networked] public float ElapsedTime { get; private set; }
    public PanelClose loadingPanel;
    public GameObject[] Enemies;
    public Transform dummyTarget;

    private bool pauseBool;

    public GameObject winPanel, losePanel;
    public TMP_Text raceTimerTxt;
    public Transform finish;

    [Networked] public bool startGame { get; set; }
    [Networked] public float gameSpeed { get; set; } = 1f;

    /// <summary>
    /// [Networked]//(OnChanged = nameof(OnLeaderboardChanged))]
    /// </summary>
    public TextMeshProUGUI leaderboardText;



    // public TextMeshProUGUI leaderboardText;
    private List<CyberNetworkController> players = new();




    public override void Spawned()
    {
        Instance = this;
        _myrunner = FindAnyObjectByType<NetworkRunner>();



        if (Object.HasStateAuthority)
        {
            // Server/host controls the start
            //StartCoroutine(StartGameRoutine());
        }




        //Enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_BoardList()
    {
        // Find players only once
        if (players.Count < 4)
        {
            players.AddRange(FindObjectsByType<CyberNetworkController>(FindObjectsSortMode.None));
        }

        // Sort players by ProgressToFinish (lower = closer)
        players.Sort((a, b) => a.ProgressToFinish.CompareTo(b.ProgressToFinish));

        // Build leaderboard string
        leaderboardText.text = "";
        for (int i = 0; i < players.Count; i++)
        {
            string name = PlayerDataManager.Instance.GetPlayerName(players[i].Object.InputAuthority);
            leaderboardText.text += $"{i + 1}:  {name}\n";
        }
    }


    public override void FixedUpdateNetwork()
    {
        // Start counting time when game starts
        if (startGame)
        {
            ElapsedTime += Runner.DeltaTime;
        }
    }

    public float GetTime()
    {
        return ElapsedTime;
    }

    public void LeaveRoom()
    {
        _myrunner.Shutdown();
    }

    public void StartRace()
    {
        startGame = true;
        Time.timeScale = 1f;
    }

    private IEnumerator StartGameRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (loadingPanel != null)
            loadingPanel.ClosePanel();

        yield return new WaitForSeconds(0.25f);
        startGame = true;
        Time.timeScale = gameSpeed;
    }

    private void Update()
    {
        if (!Object || !Runner) return;

        // This ensures all clients match time scale
        Time.timeScale = startGame ? gameSpeed : 1f;

        if (startGame)
        {
            float t = ElapsedTime;
            int minutes = Mathf.FloorToInt(t / 60f);
            int seconds = Mathf.FloorToInt(t % 60f);
            int milliseconds = Mathf.FloorToInt((t * 100f) % 100);

            raceTimerTxt.text = $"{minutes:00}:{seconds:00}.{milliseconds:00}";
        }

        if (Runner.IsServer)
        {
            RPC_BoardList();
        }

    }




    public void PauseMenu()
    {
        if (!Object.HasStateAuthority) return;

        pauseBool = !pauseBool;
        gameSpeed = pauseBool ? 0f : 1f;
        startGame = !pauseBool;
    }

    public void LoadScene(int sceneIndex)
    {

        SceneManager.LoadScene(sceneIndex);
    }


    public void ShowWinScreen(PlayerRef winner)
    {
        bool isLocalPlayerWinner = _myrunner.LocalPlayer == winner;

        if (isLocalPlayerWinner)
        {
            // Show win screen

            //UIControllerBtns.instance.ShowVictory();
            winPanel.SetActive(true);
        }
        else
        {
            // Show lose screen or just disable controls
            // UIControllerBtns.Instance.ShowDefeat();
            losePanel.SetActive(true);
        }
        _myrunner.Shutdown();

        Debug.Log($"Win screen shown. Winner: {winner}");
    }


    public void EndGame()
    {
        if (!Object.HasStateAuthority) return;

        startGame = false;
        gameSpeed = 1f;
    }

    public void MarkPlayerReady(PlayerRef player)
    {
        if (!readyPlayers.Contains(player))
        {
            readyPlayers.Add(player);
            Debug.Log($"Player {player} is ready. Total ready: {ReadyPlayerCount}");
        }
    }


    public void ResetReadyPlayers()
    {
        readyPlayers.Clear();
    }



}







public struct PlayerInputData : INetworkInput
{
    public float horizontal;
    public float vertical;
    public float rotationDelta;
    public float rotationPitchDelta;
}