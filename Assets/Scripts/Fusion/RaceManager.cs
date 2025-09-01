using UnityEngine;
using Fusion;
using TMPro;

public class RaceManager : NetworkBehaviour
{
    [Networked] private float countdown { get; set; }
    public TextMeshProUGUI countdownText;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            countdown = 3f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (countdown > 0)
        {
            countdown -= Runner.DeltaTime;
            countdownText.text = Mathf.CeilToInt(countdown).ToString();
        }
        else
        {
            countdownText.text = "GO!";
        }
    }
}