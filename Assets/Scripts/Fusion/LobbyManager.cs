using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class LobbyManager : MonoBehaviour
{
    public int maxPlayers = 1;
    public Button startButton;
    public bool go;
  public  NetworkRunner runner;
    private void Update()
    {

        // var runner = FindObjectOfType<NetworkRunner>();
        if (go)
        {
            if (runner != null && runner.SessionInfo.PlayerCount == maxPlayers)
            {
                if (runner.IsSharedModeMasterClient)
                {
                    startButton.gameObject.SetActive(true);
                }
                else
                {
                    startButton.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnStartRaceClicked()
    {
        var runner = FindObjectOfType<NetworkRunner>();
       // runner.SetActiveScene("RaceScene");
    }
}