using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using LightDI;

public class LeaderboardManager : MonoBehaviour , ISystem
{
    [Header("UI Reference (Optional)")]
    public TextMeshProUGUI leaderboardText;


    private void Awake()
    {
        InjectionManager.RegisterSystem(this);
    }


    void Start()
    {
        isLoggedIn();
    }

    #region Login

    private bool isLoggedIn()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
            return true;
        else return false;
    }

    #endregion

    #region Submit Score

    public void SubmitScore(int score)
    {
        if (!isLoggedIn())
        {
            Debug.LogWarning("User not logged in. Score not submitted.");
            return;
        }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "HighScore",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => Debug.Log("Score submitted to PlayFab!"),
            error => Debug.LogError("Error submitting score: " + error.GenerateErrorReport()));
    }

    #endregion

    #region Fetch Leaderboard

    public void GetLeaderboard()
    {
        if (!isLoggedIn())
        {
            Debug.LogWarning("Cannot fetch leaderboard. User not logged in.");
            return;
        }

        var request = new GetLeaderboardRequest
        {
            StatisticName = "HighScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnLeaderboardError);
    }

    private void OnLeaderboardSuccess(GetLeaderboardResult result)
    {
        if (leaderboardText != null)
            leaderboardText.text = "";

        foreach (var entry in result.Leaderboard)
        {
            string displayName = !string.IsNullOrEmpty(entry.DisplayName) ? entry.DisplayName : entry.PlayFabId;
            string line = $"{entry.Position + 1}. {displayName} - {entry.StatValue}";

            if (leaderboardText != null)
                leaderboardText.text += line + "\n";

            Debug.Log(line);
        }
    }

    private void OnLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Failed to load leaderboard: " + error.GenerateErrorReport());
    }

    #endregion

    #region Optional: Set Display Name

    public void SetDisplayName(string name)
    {
        if (!isLoggedIn()) return;

        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = name };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            result => Debug.Log("Display name updated: " + result.DisplayName),
            error => Debug.LogWarning("Could not set display name: " + error.GenerateErrorReport()));
    }

    #endregion
}
