using Fusion;
using TMPro;
using UnityEngine;

public class RaceCountdown : NetworkBehaviour
{
    [Networked] private float countdown { get; set; }
    public TextMeshProUGUI timerTxt;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            countdown = 3f;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && countdown > 0f)
        {
            countdown -= Runner.DeltaTime;
            if (countdown <= 0f)
            {
                countdown = 0f;
                RPC_StartRace();
            }
        }
    }

    private void Update()
    {
        if (timerTxt == null) return;

        // Read the replicated value for display
        if (countdown > 0f)
            timerTxt.text = Mathf.CeilToInt(countdown).ToString();
        else
            timerTxt.text = "GO!";
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartRace()
    {
        Debug.Log("Race starts!");
        NetworkedManager.Instance.StartRace();
        this.gameObject.SetActive(false);
    }
}
