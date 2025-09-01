using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntry : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;
    public void SetName(string n) => nameText.text = n;
    public void SetReady(bool ready) =>
        statusText.text = ready ? "Ready" : "Not Ready";


    public void SetData()
    {
        
    }



}





