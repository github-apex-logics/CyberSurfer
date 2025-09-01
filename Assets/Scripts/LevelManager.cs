using LightDI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour, ISystem
{
  

    #region Inspector References
    public PanelClose loadingPanel;
    public GameObject msg;
    public GameObject gameoverPanel;
    public GameObject gameCompletePanel;
    public RewardManager rewardManager;
 
    public GameObject[] Enemies;
    public Transform dummyTarget;

    #endregion

    #region Game State
    public bool startGame;
    public float gameSpeed = 1f;
    public bool pauseBool=false;
    public bool isGameComplete;
    public int coinCount;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        
        InjectionManager.RegisterSystem(this);
    }

    private void Start()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Enemy");
      
        coinCount = 0;
        isGameComplete = false;
        StartCoroutine(StartGame());
    }

    private void Update()
    {
        // Currently empty – can be used for input handling or other per-frame logic
    }
    #endregion

    #region Game Flow
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);
        loadingPanel.ClosePanel();
        yield return new WaitForSeconds(0.25f);

        startGame = true;
        msg.SetActive(true);
        Time.timeScale = 0; // Pause until tap
    }

    public void TapToGo()
    {
        msg.SetActive(false);
        Time.timeScale = gameSpeed;
    }

    public void PauseMenu()
    {
        pauseBool = !pauseBool;
        Time.timeScale = pauseBool ? 0 : gameSpeed;
    }

    public void GameOver()
    {
        gameoverPanel.SetActive(true);
    }


   

    public void GameComplete()
    {
        startGame = false;
        isGameComplete = true;
        Time.timeScale = 1;
        gameCompletePanel.SetActive(true);
        CompleteLevel(PlayerPrefs.GetInt("CurrentLevel"));
       
        rewardManager.GiveReward();
       
    }

    public void LoadScene()
    {
        Time.timeScale = 1;
        InjectionManager.ResetInjection();
        SceneManager.LoadScene(0); // Reloads first scene
    }
    #endregion

    #region Level Progression


    public bool IsLevelCompleted(int levelIndex)
    {
        //return PlayerPrefs.GetInt("LevelCompleted_" + levelIndex, 0) == 1;

        return Database.LevelComplete(levelIndex) == 1;

    }

    public void CompleteLevel(int levelIndex)
    {
        if (!IsLevelCompleted(levelIndex))
        {
            Database.LevelComplete(levelIndex, 1);
            Database.LevelUnlock((levelIndex + 1), 1);

            //PlayerPrefs.SetInt("LevelCompleted_" + levelIndex, 1);
            //PlayerPrefs.SetInt("LevelUnlocked_" + (levelIndex + 1), 1); // Unlock next level
        }
    }
    #endregion
}
