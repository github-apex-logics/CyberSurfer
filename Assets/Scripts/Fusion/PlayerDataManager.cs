using Fusion;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerDataManager : NetworkBehaviour
{
    public static PlayerDataManager instance;
    public PlayerDataSO playerDataSo;

    [Networked]
    private NetworkDictionary<PlayerRef, NetworkString<_32>> playerNames => default;

    private void Awakes()
    {
        if (instance == null)
        {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            //Scene targetScene = SceneManager.GetSceneByName("DontDestroyOnLoad");
            //SceneManager.MoveGameObjectToScene(gameObject,targetScene);
        }
        else
        {
            Destroy(this.gameObject);
        }
        playerDataSo.playerNames[0] = "";
        playerDataSo.playerNames[1] = "";
        playerDataSo.playerNames[2] = "";
        playerDataSo.playerNames[3] = "";

    }


    bool _isSpawned = false;

    public void MoveToScene()
    {
       
       // transform.SetParent(null);
       //// Scene targetScene = SceneManager.GetSceneByBuildIndex(6);
       // Scene targetScene = SceneManager.GetSceneByName("DontDestroyOnLoad");
       // //  if (targetScene.IsValid() && targetScene.isLoaded)
       // SceneManager.MoveGameObjectToScene(gameObject, targetScene);
    }


    public override void Spawned()
    {
        _isSpawned = true;

        Awakes();
        //if (Object.HasStateAuthority)
        //{
        //    Object.Flags |= NetworkObjectFlags.DontDestroyOnLoad();
        //    // This makes the NetworkObject survive scene changes in Fusion 2
        //}
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
            RPC_AddNamestoSO(player, name);
        }
        else
        {
            RPC_RequestSetPlayerName(player, name);
            RPC_AddNamestoSO(player,name);
        }


        //for (int i = 0; i < 4; i++)
        //{
        //    if (i == (player.PlayerId - 1))
        //    {
        //        playerDataSo.playerNames[i] = name;
        //    }

        //}
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
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_AddNamestoSO(PlayerRef player, string name)
    {

        for (int i = 0; i < 4; i++)
        {
            if (i == (player.PlayerId - 1))
            {
                playerDataSo.playerNames[i] = playerNames.ContainsKey(player).ToString();
            }

        }


        playerDataSo.playerNames[player.PlayerId - 1] = name;
    }
}
