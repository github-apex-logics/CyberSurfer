using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;


public class RewardManager : MonoBehaviour
{
    public List<RewardItem> allRewards;
    public RewardRawIUI rewardRawIUI;
    public Reward ri;

    public List<KnifeReward> consumerList;
    public List<KnifeReward> industrialList;
    public List<KnifeReward> milspecList;
    public List<KnifeReward> restrictedList;
    public List<KnifeReward> covertList;
    public List<KnifeReward> classifiedList;

    [System.Serializable]
    public class RewardProbability
    {
       [HideInInspector] public string name;
        public bool isReward; // true = rewardType, false = lootType

        public RewardType rewardType;
        [Range(0, 100)] public float reward_probability;

        public LootType lootType;
        [Range(0, 100)] public float loot_probability;

        
    }
    

    public List<RewardProbability> rewardChances;
   
    public RewardUI rewardUI;


    private void Start()
    {
        allRewards = ri.rewardItems;
        rewardRawIUI = GameObject.FindAnyObjectByType<RewardRawIUI>();
    }

    public void GiveReward()
    {
        RewardItem selectedReward = GetRandomReward();
        if (selectedReward != null)
        {
            Database.Lootbox(selectedReward.rewardType, 1);

            if (rewardRawIUI)
                rewardRawIUI.type = selectedReward.rewardType;
        }
    }
    [HideInInspector]
    public KnifeReward currentReward;

    public void GiveTierReward()
    {
        RewardItem selectedReward = GetRandomReward();
        if (selectedReward != null)
        {
            Database.Lootbox(RewardType.Casual, -1);
            switch (selectedReward.lootType)
            {
                case LootType.ConsumerGrade:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = consumerList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward = consumerList[i];
                        Debug.Log("Consumer");
                    }
                    break;
                case LootType.Mil_SpecGrade:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = milspecList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward = milspecList[i];
                        Debug.Log("milspec");
                    }
                    break;
                case LootType.Industrial:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = industrialList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward = industrialList[i];
                        Debug.Log("industy");
                    }
                    break;
                case LootType.Restricted:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = restrictedList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward = restrictedList[i];
                        Debug.Log("restricted");
                    }
                    break;
                case LootType.Covert:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = covertList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward = covertList[i];
                        Debug.Log("covert");
                    }
                    break;
                case LootType.Classified:
                    {
                        int i = Random.Range(0, consumerList.Count);
                        int index = classifiedList[i].kinfeID;
                        Database.UnlockedKnives(index, 1);
                        currentReward= classifiedList[i];
                        Debug.Log("classified");
                    }
                    break;


            }


        }
    }


    private RewardItem GetRandomReward()
    {
        float roll = Random.Range(0f, 100f);
        float current = 0f;

        foreach (var entry in rewardChances)
        {
            current += entry.isReward ? entry.reward_probability : entry.loot_probability;

            if (roll <= current)
            {
                List<RewardItem> filtered = allRewards.FindAll(r =>
                    r.isReward
                    ? r.rewardType == entry.rewardType
                    : r.lootType == entry.lootType
                );

                if (filtered.Count > 0)
                    return filtered[Random.Range(0, filtered.Count)];
            }
        }

        return null;
    }


}
