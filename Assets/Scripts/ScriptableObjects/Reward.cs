using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Reward Item")]
public class Reward : ScriptableObject
{
    public List< RewardItem> rewardItems;
}

[Serializable]
public class RewardItem
{
    public string rewardName;
    public Sprite icon;
    public RewardType rewardType; // Enum: Common, Rare, SpecialKey, etc.
    public LootType lootType;
    public int amount;
    [Tooltip("Select for Reward type not for Loot Type")]
    public bool isReward; // select weather is reward or is loot
}

[Serializable]
public class KnifeReward
{
    public string name;
    public int kinfeID;
    public Sprite icon;
}

public enum RewardType
{
    Casual,
    Rare,
    CasualKey,
    RareKey
}

public enum LootType
{
    ConsumerGrade,
    Industrial,
    Mil_SpecGrade,
    Restricted,
    Covert,
    Classified
}