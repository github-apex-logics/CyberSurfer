using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class PlayerData
{
    public int Coins;
    public long CoinsTimestamp;

    public Dictionary<int, int> LevelCompleted = new Dictionary<int, int>();
    public long LevelCompleteTimestamp;

    public int SelectedCharacter;
    public long SelectedCharacterTimestamp;

    public int SelectedKnife;
    public long SelectedKnifeTimestamp;

    public Dictionary<int, int> UnlockedCharacters = new Dictionary<int, int>();
    public long UnlockedCharactersTimestamp;

    public Dictionary<int, int> UnlockedKnives = new Dictionary<int, int>();
    public long UnlockedKnivesTimestamp;

    public Dictionary<int, float> BestRunTimes = new Dictionary<int, float>();
    public Dictionary<int, long> BestRunTimesTimestamps = new Dictionary<int, long>();

    public Dictionary<string, int> Lootboxes = new Dictionary<string, int>();
    public Dictionary<string, long> LootboxesTimestamps = new Dictionary<string, long>();
}

public class PlayFabSyncManager : MonoBehaviour
{
    public static PlayFabSyncManager Instance;
    public PlayerData playerData;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //LoadLocalData();
    }

    

    /// <summary>
    /// Load all local PlayerPrefs data into PlayerData
    /// </summary>
    public void LoadLocalData()
    {
        playerData = new PlayerData();

        playerData.Coins = Database.Coins;
        playerData.CoinsTimestamp = Database.CoinsTimestamp;

        // Levels completed: iterate through saved keys (example assumes max 100 levels)
        for (int i = 1; i <= 100; i++)
        {
            int completed = Database.LevelComplete(i);
            if (completed > 0) playerData.LevelCompleted[i] = completed;
        }
        playerData.LevelCompleteTimestamp = Database.LevelCompleteTimestamp;

        playerData.SelectedCharacter = Database.SelectedCharacter;
        playerData.SelectedCharacterTimestamp = Database.SelectedCharacterTimestamp;

        playerData.SelectedKnife = Database.SelectedKnife;
        playerData.SelectedKnifeTimestamp = Database.SelectedKnifeTimestamp;

        // Unlocked Characters
        for (int i = 1; i <= 100; i++)
        {
            int unlocked = Database.UnlockedCharacters(i);
            if (unlocked > 0) playerData.UnlockedCharacters[i] = unlocked;
        }
        playerData.UnlockedCharactersTimestamp = Database.UnlockedCharactersTimestamp;

        // Unlocked Knives
        for (int i = 1; i <= 100; i++)
        {
            int unlocked = Database.UnlockedKnives(i);
            if (unlocked > 0) playerData.UnlockedKnives[i] = unlocked;
        }
        playerData.UnlockedKnivesTimestamp = Database.UnlockedKnivesTimestamp;

        // Best run times
        for (int i = 1; i <= 100; i++)
        {
            float time = Database.BestRunTime(i);
            if (time > 0)
            {
                playerData.BestRunTimes[i] = time;
                playerData.BestRunTimesTimestamps[i] = long.Parse(Database.GetBestRunTimeTimestamp(i));
            }
        }

        // Lootboxes (Casual, Rare)
        foreach (RewardType type in Enum.GetValues(typeof(RewardType)))
        {
            string key = type.ToString();
            int count = Database.Lootbox(type);
            playerData.Lootboxes[key] = count;
            playerData.LootboxesTimestamps[key] = long.Parse(Database.GetLootboxTimestamp(type));
        }
    }

    /// <summary>
    /// Initiate syncing with PlayFab
    /// </summary>
    public void SyncData()
    {
        Debug.Log("Starting PlayFab sync...");
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnPlayFabError);
    }

    private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        Debug.Log("Fetched PlayFab data, merging with local data...");

        playerData.Coins = ResolveConflict("Coins", playerData.Coins, playerData.CoinsTimestamp, result);

        playerData.LevelCompleted = ResolveJsonDictConflict<int, int>(
            "LevelCompleted", "LevelCompleteTimestamp", playerData.LevelCompleted,
            new Dictionary<int, long> { { -1, playerData.LevelCompleteTimestamp } }, result);

        playerData.SelectedCharacter = ResolveConflict("SelectedCharacter", playerData.SelectedCharacter, playerData.SelectedCharacterTimestamp, result);
        playerData.SelectedKnife = ResolveConflict("SelectedKnife", playerData.SelectedKnife, playerData.SelectedKnifeTimestamp, result);

        playerData.UnlockedCharacters = ResolveJsonDictConflict<int, int>(
            "UnlockedCharacters", "UnlockedCharactersTimestamp", playerData.UnlockedCharacters,
            new Dictionary<int, long> { { -1, playerData.UnlockedCharactersTimestamp } }, result);

        playerData.UnlockedKnives = ResolveJsonDictConflict<int, int>(
            "UnlockedKnives", "UnlockedKnivesTimestamp", playerData.UnlockedKnives,
            new Dictionary<int, long> { { -1, playerData.UnlockedKnivesTimestamp } }, result);

        playerData.BestRunTimes = ResolveJsonDictConflict<int, float>(
            "BestRunTimes", "BestRunTimesTimestamps", playerData.BestRunTimes, playerData.BestRunTimesTimestamps, result);

        playerData.Lootboxes = ResolveJsonDictConflict<string, int>(
            "Lootboxes", "LootboxesTimestamps", playerData.Lootboxes, playerData.LootboxesTimestamps, result);

        SaveToPlayFab();
    }

    private int ResolveConflict(string key, int localValue, long localTs, GetUserDataResult result)
    {
        int remoteValue = localValue;
        long remoteTs = 0;

        if (result.Data != null)
        {
            if (result.Data.ContainsKey(key))
                int.TryParse(result.Data[key].Value, out remoteValue);
            if (result.Data.ContainsKey(key + "Timestamp"))
                long.TryParse(result.Data[key + "Timestamp"].Value, out remoteTs);
        }

        return (localTs > remoteTs) ? localValue : remoteValue;
    }

    private Dictionary<TKey, TValue> ResolveJsonDictConflict<TKey, TValue>(
        string valueKey, string tsKey,
        Dictionary<TKey, TValue> localValues,
        Dictionary<TKey, long> localTimestamps,
        GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey(valueKey) || !result.Data.ContainsKey(tsKey))
            return localValues;

        try
        {
            var remoteValues = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(result.Data[valueKey].Value);
            var remoteTimestamps = JsonConvert.DeserializeObject<Dictionary<TKey, long>>(result.Data[tsKey].Value);

            foreach (var kvp in remoteValues)
            {
                long remoteTs = remoteTimestamps.ContainsKey(kvp.Key) ? remoteTimestamps[kvp.Key] : 0;
                long localTs = localTimestamps.ContainsKey(kvp.Key) ? localTimestamps[kvp.Key] : 0;

                if (remoteTs > localTs)
                {
                    localValues[kvp.Key] = kvp.Value;
                    localTimestamps[kvp.Key] = remoteTs;
                }
            }
        }
        catch
        {
            Debug.LogWarning($"Failed to parse JSON for {valueKey}/{tsKey}");
        }

        return localValues;
    }

    public void SaveToPlayFab()
    {
        Debug.Log("Saving merged data to PlayFab...");

        List<Dictionary<string, string>> batches = new List<Dictionary<string, string>>();

        // Batch 1: simple scalar fields
        batches.Add(new Dictionary<string, string>
        {
            { "Coins", playerData.Coins.ToString() },
            { "CoinsTimestamp", playerData.CoinsTimestamp.ToString() },
            { "SelectedCharacter", playerData.SelectedCharacter.ToString() },
            { "SelectedCharacterTimestamp", playerData.SelectedCharacterTimestamp.ToString() },
            { "SelectedKnife", playerData.SelectedKnife.ToString() },
            { "SelectedKnifeTimestamp", playerData.SelectedKnifeTimestamp.ToString() }
        });

        // Batch 2: JSON dictionaries
        batches.Add(new Dictionary<string, string>
        {
            { "LevelCompleted", JsonConvert.SerializeObject(playerData.LevelCompleted) },
            { "LevelCompleteTimestamp", playerData.LevelCompleteTimestamp.ToString() },
            { "UnlockedCharacters", JsonConvert.SerializeObject(playerData.UnlockedCharacters) },
            { "UnlockedCharactersTimestamp", playerData.UnlockedCharactersTimestamp.ToString() },
            { "UnlockedKnives", JsonConvert.SerializeObject(playerData.UnlockedKnives) },
            { "UnlockedKnivesTimestamp", playerData.UnlockedKnivesTimestamp.ToString() },
            { "BestRunTimes", JsonConvert.SerializeObject(playerData.BestRunTimes) },
            { "BestRunTimesTimestamps", JsonConvert.SerializeObject(playerData.BestRunTimesTimestamps) },
            { "Lootboxes", JsonConvert.SerializeObject(playerData.Lootboxes) },
            { "LootboxesTimestamps", JsonConvert.SerializeObject(playerData.LootboxesTimestamps) }
        });

        SendBatchesSequentially(batches, 0);
    }

    private void SendBatchesSequentially(List<Dictionary<string, string>> batches, int index)
    {
        if (index >= batches.Count)
        {
            Debug.Log("All PlayFab data uploaded successfully.");
            return;
        }

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = batches[index]
        },
        res =>
        {
            Debug.Log($"Batch {index + 1} saved.");
            SendBatchesSequentially(batches, index + 1);
        },
        err => Debug.LogError($"PlayFab error on batch {index + 1}: {err.GenerateErrorReport()}"));
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.LogError("PlayFab error: " + error.GenerateErrorReport());
    }
}
