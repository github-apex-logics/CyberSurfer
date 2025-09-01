using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager Instance;
    public PlayerDataSO playerDataSo;

    [Networked]
    private NetworkDictionary<PlayerRef, NetworkString<_32>> playerNames => default;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }


    }


    bool _isSpawned = false;

    public override void Spawned()
    {
        _isSpawned = true;
    }


    public void SetPlayerName(PlayerRef player, string name)
    {
        if (!_isSpawned)
        {
            Debug.LogWarning("PlayerDataManager is not yet spawned. Delaying name set.");
            return;
        }

        if (Runner.IsServer)
        {
            playerNames.Set(player, name);
        }
        else
        {
            RPC_RequestSetPlayerName(player, name);
        }


        for (int i = 0; i < 4; i++)
        {
            if (i == player.PlayerId)
            {
                playerDataSo.playerNames[i] = name;
            }
        
        }
    }


    public string GetPlayerName(PlayerRef player)
    {
        if (playerNames.TryGet(player, out var name))
            return name.ToString();

        return $"Player {player.PlayerId}";
    }


    public void DebugAllPlayerNames()
    {
        Debug.Log("Listing all player names:");

        foreach (var kvp in playerNames)
        {
            PlayerRef player = kvp.Key;
            string name = kvp.Value.ToString();

            Debug.Log($"PlayerRef: {player} | Name: {name}");
        }
    }




    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_RequestSetPlayerName(PlayerRef player, string name)
    {
        playerNames.Set(player, name);
    }
}
