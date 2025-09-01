using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class LeaderboardManager1 : MonoBehaviour
{
    [Header("Leaderboard Settings")]
    [Tooltip("Statistic key name for this leaderboard (set in PlayFab).")]
    public string[] statisticKey;//= "HighScore";



    [Tooltip("How many results to fetch.")]
    public int maxResultsCount = 10;

    [Header("UI References")]
    public Transform leaderboardContainer; // Parent UI container
    public GameObject leaderboardEntryPrefab; // Prefab with TMP Texts for Rank, Name, Score

     float timeMins;
    public int timeMs;
    public string result;

    public void Start()
    {
        timeMins = float.Parse("00.57");

        timeMs = ConvertInt(timeMins);
        result = FormatTimeMs(ConvertInt(timeMins));

        UpdatePlayerScore(0.57f, 2);
    }




    // Clears old leaderboard entries before showing new ones
    private void ClearLeaderboardUI()
    {
        foreach (Transform child in leaderboardContainer)
            Destroy(child.gameObject);
    }



    #region Login

    private bool isLoggedIn()
    {
        if (PlayFabClientAPI.IsClientLoggedIn())
            return true;
        else return false;
    }

    #endregion
    // Public function -> just pass the statistic key and call it
    //public void GetLeaderboard(string key = null)
    //{
    //    if (isLoggedIn())
    //    {
    //        if (!string.IsNullOrEmpty(key))
    //            statisticKey = key; // Override if passed dynamically

    //        var request = new GetLeaderboardRequest
    //        {
    //            StatisticName = statisticKey,
    //            StartPosition = 0,
    //            MaxResultsCount = maxResultsCount
    //        };

    //        PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnLeaderboardError);
    //    }
    //}



    public void GetLeaderboard(int num)
    {
        if (isLoggedIn())
        {
            
               // statisticKey = key.ToString(); // Override if passed dynamically

            var request = new GetLeaderboardRequest
            {
                StatisticName = statisticKey[num],
                StartPosition = 0,
                MaxResultsCount = maxResultsCount
            };

            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardSuccess, OnLeaderboardError);
        }
    }



  


    private void OnLeaderboardSuccess(GetLeaderboardResult result)
    {
       
            Debug.Log($" Leaderboard {statisticKey} fetched, {result.Leaderboard.Count} entries.");
            ClearLeaderboardUI();

            foreach (var entry in result.Leaderboard)
            {
                // Spawn UI entry
                GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);

                // Assuming prefab has children: RankText, NameText, ScoreText
                TextMeshProUGUI[] texts = entryObj.GetComponentsInChildren<TextMeshProUGUI>();
                texts[0].text = (entry.Position + 1).ToString();   // Rank
                texts[1].text = entry.DisplayName ?? "Anonymous";  // Name
            texts[2].text = FormatTimeMs(entry.StatValue);//  entry.StatValue.ToString();        // Score
            }
        
    }

    private void OnLeaderboardError(PlayFabError error)
    {
        ClearLeaderboardUI();
        Debug.LogError($" Failed to load leaderboard {statisticKey}: {error.GenerateErrorReport()}");
    }

    // Example: Call after player finishes a run
    public void UpdatePlayerScore(float score, int num)
    {
        if (isLoggedIn())
        {
            
            var request = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = statisticKey[num],
                    Value = ConvertInt(score)
                }
            }
            };

            PlayFabClientAPI.UpdatePlayerStatistics(request,
                result =>
                {
                    Debug.Log($" Score {score} saved in {statisticKey}");
                   // GetLeaderboard(num); // Refresh UI
                },
                error => Debug.LogError($" Failed to update score: {error.GenerateErrorReport()}")
            );
        }
    }






    /// <summary>
    /// convert int to string
    /// </summary>
    /// <returns></returns>

    public int ConvertInt( float timeMins)
    {

        int minutes = Mathf.FloorToInt(timeMins);
        float remaining = (timeMins - minutes) * 100; // turn decimal into "seconds"
        int seconds = Mathf.RoundToInt(remaining);

        return ToMilliseconds(minutes, seconds);
        

        


    }
    public int ToMilliseconds(int minutes, int seconds, int milliseconds = 0)
    {
        return (minutes * 60 * 1000) + (seconds * 1000) + milliseconds;
    }

    public string FormatTimeMs(int ms)
    {

        int minutes = ms / 60000;
        int seconds = (ms / 1000) % 60;
        int milliseconds = (ms % 1000) / 10; // 2-digit precision

        return $"{minutes:00}:{seconds:00}";
    }







}

