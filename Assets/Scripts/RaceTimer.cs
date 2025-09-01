using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;

public class RaceTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI raceTimeText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI coinText;
    LeaderboardManager1 lb;

    [Header("Settings")]
    public string bestTimeKey = "BestTime";

    private float currentTime = 0f;
    private float bestTime = 0;


    [Header("References")]
    public LevelManager levelManager;

    void Start()
    {
        lb = FindAnyObjectByType<LeaderboardManager1>();
        StartTimer();
        LoadBestTime();
        // UpdateUI();

        Debug.LogError(ConvertedScore(0.57f));


    }

    void Update()
    {
        // Auto start when LevelManager.startGame becomes true
        if (levelManager != null && levelManager.startGame)
        {
            currentTime += Time.deltaTime;
            UpdateCurrentTimeUI();
           // CheckBestTime();
            
        }
        if (levelManager.isGameComplete)
        {
            CheckBestTime();
        }

      
    }

    public void StartTimer()
    {
        currentTime = 0f;

    }



    private void CheckBestTime()
    {
        // If no best time has ever been saved OR current run is better
        if (bestTime == 0 || currentTime < bestTime)
        {
            bestTime = currentTime;
            Database.BestRunTime(PlayerPrefs.GetInt("CurrentLevel"), bestTime);
            PlayerPrefs.SetFloat(bestTimeKey + PlayerPrefs.GetInt("CurrentLevel"), bestTime);
            PlayerPrefs.Save();
            UpdateBestTimeUI();

            if (lb)
                lb.UpdatePlayerScore(ConvertedScore(), (PlayerPrefs.GetInt("CurrentLevel") - 1));
        }
    }



    float ConvertedScore()
    {
        string s = FormatTime(bestTime);  
        string[] parts = s.Split(':');    

       


        if (parts.Length == 2)
        {
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);
           

            float totalSeconds = minutes * 60 + seconds;
           //\ Debug.LogError("Converted: " + totalSeconds);

            return totalSeconds;
        }
        else
            return 0f;
    }
    float ConvertedScore(float b)
    {
        string s = FormatTime(b);
        string[] parts = s.Split(':');




        if (parts.Length == 2)
        {
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);


            float totalSeconds = minutes * 60 + seconds;
            //\ Debug.LogError("Converted: " + totalSeconds);

            return totalSeconds;
        }
        else
            return 0f;
    }

    private void LoadBestTime()
    {
        bestTime = PlayerPrefs.GetFloat(bestTimeKey + PlayerPrefs.GetInt("CurrentLevel"), 0);
    }

    private void UpdateUI()
    {
        UpdateCurrentTimeUI();
        UpdateBestTimeUI();
    }

    private void UpdateCurrentTimeUI()
    {
        if (raceTimeText != null)
            raceTimeText.text =  FormatTime(currentTime);
        currentTimeText.text = raceTimeText.text;
        coinText.text = levelManager.coinCount.ToString();
    }

   private void UpdateBestTimeUI()
{
    if (bestTimeText != null)
    {
        if (bestTime == 0)
            bestTimeText.text = "Best: --:--"; // first time no best yet
        else
            bestTimeText.text =  FormatTime(bestTime);
    }
}

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        // int milliseconds = Mathf.FloorToInt((time * 1000f) % 1000f);
        return $"{minutes:00}:{seconds:00}";//:{milliseconds:000}";
    }
}
