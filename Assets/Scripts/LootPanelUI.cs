
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootPanelUI : MonoBehaviour
{
    public List<Transform> items;
    public RewardManager lootManager;
    public GameObject lootBoxAnimation, casualLootbox, rareLootbox;
    public GameObject errorMsg;
    private void Awake()
    {
        foreach (Transform t in this.transform)
        {
            items.Add(t);
        }
        
    }

    public void Test(int i)
    {
        if (i == 1)
            Database.Lootbox(RewardType.Casual, 1);
        if (i == 2)
            Database.Lootbox(RewardType.Rare, 1);
        if (i == 3)
            Database.Lootbox(RewardType.CasualKey, 1);
        if (i == 4)
            Database.Lootbox(RewardType.RareKey, 1);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        
        UpdateList();
    }

    // Update is called once per frame

    public void UpdateList()
    {
        UpdateLootboxUI(0, RewardType.Casual);
        UpdateLootboxUI(1, RewardType.Rare);
        UpdateLootboxUI(2, RewardType.CasualKey);
        UpdateLootboxUI(3, RewardType.RareKey);
    }

    private void UpdateLootboxUI(int index, RewardType type)
    {
        int count = Database.Lootbox(type);
        Transform item = items[index];

        bool hasLootbox = count >= 1;

        item.GetChild(3).gameObject.SetActive(!hasLootbox);
        item.GetChild(5).gameObject.SetActive(!hasLootbox);
        item.GetChild(4).gameObject.SetActive(hasLootbox);
        item.GetChild(6).gameObject.SetActive(!hasLootbox);
        item.GetComponent<Button>().enabled = hasLootbox;

        if (hasLootbox)

            item.GetChild(4).GetComponent<TMP_Text>().text = "x " + count;
    }


    public void OpenBox(int i)
    {
        if (i == 1 && Database.Lootbox(RewardType.CasualKey) >= 1)
        {
            Database.Lootbox(RewardType.Casual, -1);
            Database.Lootbox(RewardType.CasualKey, -1);
            lootManager.GiveTierReward();
            lootBoxAnimation.gameObject.SetActive(true);
            lootBoxAnimation.transform.GetChild(2).transform.GetChild(1).GetComponent<Image>().sprite = lootManager.currentReward.icon;
            casualLootbox.gameObject.SetActive(true);
        }
        else
        {
            errorMsg.SetActive(true);
        }
        if (i == 2 && Database.Lootbox(RewardType.RareKey) >= 1)
        {
            Database.Lootbox(RewardType.Rare, -1);
            Database.Lootbox(RewardType.RareKey, -1);
            lootManager.GiveTierReward();
            lootBoxAnimation.gameObject.SetActive(true);
            lootBoxAnimation.transform.GetChild(2).transform.GetChild(1).GetComponent<Image>().sprite = lootManager.currentReward.icon;
            rareLootbox.gameObject.SetActive(true);
        }
        else
        {
            errorMsg.SetActive(true);
        }

        UpdateList();
    }

    public void Test()
    {
        Database.Lootbox(RewardType.CasualKey, 1);
        UpdateList();
    }

}
