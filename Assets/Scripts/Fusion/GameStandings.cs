using TMPro;
using UnityEngine;

public class GameStandings : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text[] userName, userScore;

    public string[] playerName = new string[4], playerScore = new string[4];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }


    private void OnEnable()
    {
        UpdateBoard();  
    }

    void UpdateBoard()
    {
        for (int i = 0; i < 4; i++)
        {
            if (userName[i].text == string.Empty)
            {
                userName[i].gameObject.transform.parent.gameObject.SetActive(false);
                userName[i].text = playerName[i];

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
