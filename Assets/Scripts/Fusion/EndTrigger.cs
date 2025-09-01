using Fusion;
using UnityEngine;

public class EndTrigger : NetworkBehaviour
{
    private bool winnerDeclared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!HasStateAuthority || winnerDeclared) return;

        // Make sure only player objects trigger this
        if (other.TryGetComponent<CyberNetworkController>(out var player))
        {
            winnerDeclared = true;
            Debug.Log($"Winner: {player.Object.InputAuthority}");

            RPC_DeclareFirstWinner(player.Object.InputAuthority);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DeclareFirstWinner(PlayerRef winner)
    {
        Debug.Log($"[RPC] Player {winner} is the winner!");

        NetworkedManager.Instance.ShowWinScreen(winner);
    }
}
