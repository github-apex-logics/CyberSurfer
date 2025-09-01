using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<RewardItem> inventory = new List<RewardItem>();
    public List<RewardItem> equippedInventoryItems = new List<RewardItem>();
    public Reward ri;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    private void Start()
    {
        
        LoadEquippedInventory();
    }

    public void AddToInventory(RewardItem reward)
    {
        // Optional: check duplicates, increase amount, etc.
        inventory.Add(reward);
        PlayerPrefs.SetInt("Inventory_" + reward.rewardName,
            PlayerPrefs.GetInt("Inventory_" + reward.rewardName, 0) + reward.amount);
    }

    public int GetRewardCount(string rewardName)
    {
        return PlayerPrefs.GetInt("Inventory_" + rewardName, 0);
    }



    public List<RewardItem> GetAllEquippedItems()
    {
        return new List<RewardItem>(equippedInventoryItems);
    }

    private void LoadEquippedInventory()
    {
        equippedInventoryItems.Clear();

        for (int i = 0; i < ri.rewardItems.Count; i++)
        {
            GetAllEquippedItems().Add(ri.rewardItems[i]);
        }

        LoadFromPlayerPrefs(equippedInventoryItems, "", "Inventory_");
    }

    private void LoadFromPlayerPrefs(List<RewardItem> targetList, string keysKey, string prefix)
    {
        string savedKeys = PlayerPrefs.GetString(keysKey, "");
        if (!string.IsNullOrEmpty(savedKeys))
        {
            string[] keys = savedKeys.Split('|');
            foreach (string key in keys)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    int count = PlayerPrefs.GetInt(prefix + key, 0);
                    if (count > 0)
                    {
                        targetList.Add(new RewardItem { rewardName = key, amount = count });
                    }
                }
            }
        }
    }

    private void AddOrUpdateLocalList(List<RewardItem> list, RewardItem reward, string keysKey)
    {
        var existing = list.Find(i => i.rewardName == reward.rewardName);
        if (existing != null)
        {
            existing.amount += reward.amount;
        }
        else
        {
            list.Add(new RewardItem { rewardName = reward.rewardName, amount = reward.amount });

            // Save key to PlayerPrefs key list
            string savedKeys = PlayerPrefs.GetString(keysKey, "");
            if (!savedKeys.Contains(reward.rewardName))
            {
                if (!string.IsNullOrEmpty(savedKeys))
                    savedKeys += "|";
                savedKeys += reward.rewardName;
                PlayerPrefs.SetString(keysKey, savedKeys);
                PlayerPrefs.Save();
            }
        }
    }
}
