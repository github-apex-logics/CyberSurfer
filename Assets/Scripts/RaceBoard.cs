using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;

public class RaceLeaderboard : MonoBehaviour
{
    public TextMeshProUGUI leaderboardText;
    private List<CyberNetworkController> players = new();

    void Update()
    {
        // Find players only once
        if (players.Count < 2)
        {
            players.AddRange(FindObjectsOfType<CyberNetworkController>());
        }

        // Sort players by ProgressToFinish (lower = closer)
        players.Sort((a, b) => a.ProgressToFinish.CompareTo(b.ProgressToFinish));

        // Build leaderboard string
        leaderboardText.text = "";
        for (int i = 0; i < players.Count; i++)
        {
            string name = players[i].Object.InputAuthority.ToString(); // Replace with actual name if available
            leaderboardText.text += $"{i + 1}. Player {name}\n";
        }
    }
}
