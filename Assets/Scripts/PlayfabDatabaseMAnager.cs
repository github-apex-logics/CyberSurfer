using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayFabDatabaseManager : MonoBehaviour
{
    private const string DATABASE_KEY = "GameDatabase";

    public void SaveDatabase(GameDatabase db)
    {
        var json = GameDatabaseSerializer.Serialize(db);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string> { { DATABASE_KEY, json } }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Game data saved successfully."),
            error => Debug.LogError("Failed to save data: " + error.GenerateErrorReport()));
    }

    public void LoadDatabase(Action<GameDatabase> onComplete)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey(DATABASE_KEY))
                {
                    string json = result.Data[DATABASE_KEY].Value;
                    GameDatabase db = GameDatabaseSerializer.Deserialize(json);
                    onComplete?.Invoke(db);
                }
                else
                {
                    Debug.Log("No saved data found, creating new.");
                    onComplete?.Invoke(new GameDatabase());
                }
            },
            error => Debug.LogError("Failed to load data: " + error.GenerateErrorReport()));
    }
}



public static class GameDatabaseSerializer
{
    public static string Serialize(GameDatabase db)
    {
        return JsonUtility.ToJson(db);
    }

    public static GameDatabase Deserialize(string json)
    {
        return JsonUtility.FromJson<GameDatabase>(json);
    }
}




public static class PlayFabDataHandler
{
    public static void SaveGameData(GameDatabase data)
    {
        string json = JsonUtility.ToJson(data);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "GameDatabase", json }
            }
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("Game data saved successfully."),
            error => Debug.LogError("Failed to save data: " + error.GenerateErrorReport()));
    }

    public static void LoadGameData(System.Action<GameDatabase> onComplete)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("GameDatabase"))
                {
                    string json = result.Data["GameDatabase"].Value;
                    GameDatabase db = JsonUtility.FromJson<GameDatabase>(json);
                    onComplete?.Invoke(db);
                }
                else
                {
                    Debug.Log("No game data found. Creating new data.");
                    onComplete?.Invoke(new GameDatabase());
                }
            },
            error =>
            {
                Debug.LogError("Failed to load data: " + error.GenerateErrorReport());
                onComplete?.Invoke(null);
            });
    }
}

