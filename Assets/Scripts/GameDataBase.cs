using System.Collections.Generic;
using System;

[System.Serializable]
public class GameDatabase
{
    public List<CharacterData> characters = new();
    public List<WeaponData> weapons = new();
    public List<LootBoxData> lootBoxes = new();
    public List<KeyData> keys = new();

    public string equippedCharacterId;
    public string equippedWeaponId;

    public int coins;
    public int gems;
}

[Serializable]
public class PlayerProgress
{
    public int currentLevel;
    public int coins;
    public int gems;
    public List<int> completedLevels = new();
}
[Serializable]
public class PlayerInventory
{
    public List<string> ownedCharacters = new();
    public List<string> ownedSkins = new();
    public Dictionary<string, int> lootBoxes = new(); // e.g. {"casual": 3, "rare": 1}
    public List<string> specialKeys = new(); // If keys are unique, otherwise use a count
}

[Serializable]
public class PlayerSettings
{
    public float soundVolume = 1f;
    public float musicVolume = 1f;
    public bool vibrationEnabled = true;
}

[System.Serializable]
public class CharacterData
{
    public string characterId;
    public bool isUnlocked;
    public int level;
    public string equippedSkinId;
}

[System.Serializable]
public class WeaponData
{
    public string weaponId;
    public bool isUnlocked;
    public int upgradeLevel;
}

[System.Serializable]
public class LootBoxData
{
    public string lootBoxId;
    public int count;
}

[System.Serializable]
public class KeyData
{
    public string keyId;
    public int count;
}
