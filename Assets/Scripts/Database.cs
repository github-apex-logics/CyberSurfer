using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.Rendering.DebugUI;

public class Database
{
    // ------------------- Coins -------------------
    public static int Coins
    {
        get => PlayerPrefs.GetInt("Coins", 30) ;
        set
        {
            PlayerPrefs.SetInt("Coins", value);
            CoinsTimestamp = DateTime.UtcNow.Ticks;
            PlayerPrefs.Save();
        }
    }
    public static long CoinsTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("CoinsTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("CoinsTimestamp", value.ToString());
    }

    // ------------------- Levels Completed -------------------
   
    public static int LevelComplete(int index)
    {
        return (PlayerPrefs.GetInt("LevelCompleted_" + index, 0));
        
    }
    public static void LevelComplete(int index, int value)
    {
        PlayerPrefs.SetInt("LevelCompleted_" + index, value);
        LevelCompleteTimestamp = DateTime.UtcNow.Ticks;
        PlayerPrefs.Save();

    }




    public static long LevelCompleteTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("LevelCompleteTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("LevelCompleteTimestamp", value.ToString());
    }


    //-------------------Level Unlocked----------------------


    public static int LevelUnlock(int index)
    {
        return (PlayerPrefs.GetInt("LevelUnlocked_" + index, 0));

    }
    public static void LevelUnlock(int index, int value)
    {
        PlayerPrefs.SetInt("LevelUnlocked_" + index, value);
        LevelUnlockTimestamp = DateTime.UtcNow.Ticks;
        PlayerPrefs.Save();

    }

    public static long LevelUnlockTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("LevelUnlockTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("LevelUnlockTimestamp", value.ToString());
    }

    //-------------------------------------------------------
    // ------------------- Selected Character -------------------
    public static int SelectedCharacter
    {
        get => (PlayerPrefs.GetInt("SelectedCharacter", 1));
        set
        {
            PlayerPrefs.SetInt("SelectedCharacter", value);
            SelectedCharacterTimestamp = DateTime.UtcNow.Ticks;
            PlayerPrefs.Save();
        }
    }
    public static long SelectedCharacterTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("SelectedCharacterTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("SelectedCharacterTimestamp", value.ToString());
    }

    // ------------------- Selected Knife -------------------
    public static int SelectedKnife
    {
        get => PlayerPrefs.GetInt("SelectedKnife", 1);
        set
        {
            PlayerPrefs.SetInt("SelectedKnife", value);
            SelectedKnifeTimestamp = DateTime.UtcNow.Ticks;
            PlayerPrefs.Save();
        }
    }
    public static long SelectedKnifeTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("SelectedKnifeTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("SelectedKnifeTimestamp", value.ToString());
    }

    // ------------------- Unlocked Characters -------------------
    public static int UnlockedCharacters(int id)
    {
        return (PlayerPrefs.GetInt("UnlockedCharacters" + id, 1));
       
    }
    public static void UnlockedCharacters(int id, int value)
    {
       PlayerPrefs.SetInt("UnlockedCharacters" + id, value);
       UnlockedCharactersTimestamp = DateTime.UtcNow.Ticks;
       PlayerPrefs.Save();
        
    }
    
    public static long UnlockedCharactersTimestamp 
    {
        get => long.Parse(PlayerPrefs.GetString("UnlockedCharactersTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("UnlockedCharactersTimestamp", value.ToString());
    }

    // ------------------- Unlocked Knives -------------------
    public static int UnlockedKnives(int id)
    {
        return (PlayerPrefs.GetInt("UnlockedKnives" + id, 0));
    }
    public static void UnlockedKnives(int id, int value)
    {
        PlayerPrefs.SetInt("UnlockedKnives" + id, value);
        UnlockedKnivesTimestamp = DateTime.UtcNow.Ticks;
        PlayerPrefs.Save();
    }

    public static long UnlockedKnivesTimestamp
    {
        get => long.Parse(PlayerPrefs.GetString("UnlockedKnivesTimestamp", DateTime.UtcNow.Ticks.ToString()));
        private set => PlayerPrefs.SetString("UnlockedKnivesTimestamp", value.ToString());
    }

    // ------------------- Best Run Time per Level -------------------
    public static float BestRunTime(int level)
    {
        return (PlayerPrefs.GetFloat("BestRunTime" + level, 0));
    }
    public static void BestRunTime(int level, float time)
    {
        PlayerPrefs.SetFloat("BestRunTime" + level, time);
        PlayerPrefs.SetString($"BestRunTime_Level_{level}_Timestamp", DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.Save();
    }
    public static string GetBestRunTimeTimestamp(int level)
    {
        return PlayerPrefs.GetString($"BestRunTime_Level_{level}_Timestamp", DateTime.UtcNow.Ticks.ToString());
    }

    // ------------------- Lootboxes -------------------
    public static int Lootbox(RewardType type)
    {
        return (PlayerPrefs.GetInt("Lootbox" + type, 0));
    }
    public static void Lootbox(RewardType type, int value)
    {
        PlayerPrefs.SetInt("Lootbox" + type, Lootbox(type) + value);
        PlayerPrefs.SetString($"Lootbox_{type}_Timestamp", DateTime.UtcNow.Ticks.ToString());
        PlayerPrefs.Save();
    }
    public static string GetLootboxTimestamp(RewardType type)
    {
        return PlayerPrefs.GetString($"Lootbox_{type}_Timestamp", DateTime.UtcNow.Ticks.ToString());
    }

   
}


