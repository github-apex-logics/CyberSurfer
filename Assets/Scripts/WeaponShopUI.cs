using Coffee.UIEffects;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopUI : MonoBehaviour
{
    public WeaponDatabaseSO WeaponDatabaseSO;
    
   public List< Transform> items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // reading from  a scriptable object and update the UI objects by accessing the child
    //also checking the lock unclock state and enabling effect according to that
    
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            int index = i; // capture index
            Transform t = transform.GetChild(i);

            t.GetComponent<Button>().onClick.AddListener(() => SelectKnife(index + 1));
            items.Add(t);
        }

      
    }
    private void OnEnable()
    {
        LoadWeapons();
       
    }

    void UpdateList()
    {

        for (int i = 0; i < items.Count; i++)
        {
            items[i].GetChild(1).GetComponent<Image>().sprite = WeaponDatabaseSO.weapons[i].icon;
            items[i].GetChild(3).GetComponent<TMP_Text>().text = WeaponDatabaseSO.weapons[i].weaponName;
            items[i].GetChild(4).GetComponent<TMP_Text>().text = WeaponDatabaseSO.weapons[i].price.ToString();
            items[i].GetChild(0).GetComponent<UIEffect>().effectMode = EffectMode.Grayscale;
            items[i].GetChild(2).GetComponent<UIEffect>().effectMode = EffectMode.Grayscale;
            items[i].GetChild(1).GetComponent<UIShiny>().enabled = false;
            items[i].GetChild(7).gameObject.SetActive(false);
            if (WeaponDatabaseSO.weapons[i].state == WeaponState.Purchased)
            {
                items[i].GetChild(4).gameObject.SetActive(false);
                items[i].GetChild(3).gameObject.SetActive(false);
                items[i].GetChild(6).gameObject.SetActive(false);
                items[i].GetChild(7).gameObject.SetActive(false);
                items[i].GetChild(5).GetComponent<TMP_Text>().text = WeaponDatabaseSO.weapons[i].weaponName;
                items[i].GetChild(0).GetComponent<UIEffect>().effectMode = EffectMode.None;
                items[i].GetChild(2).GetComponent<UIEffect>().effectMode = EffectMode.None;
            }
            if (WeaponDatabaseSO.weapons[i].state == WeaponState.Selected)
            {
                items[i].GetChild(4).gameObject.SetActive(false);
                items[i].GetChild(3).gameObject.SetActive(false);
                items[i].GetChild(6).gameObject.SetActive(false);
                items[i].GetChild(7).gameObject.SetActive(true);
                items[i].GetChild(5).GetComponent<TMP_Text>().text = WeaponDatabaseSO.weapons[i].weaponName;
                items[i].GetChild(0).GetComponent<UIEffect>().effectMode = EffectMode.None;
                items[i].GetChild(2).GetComponent<UIEffect>().effectMode = EffectMode.None;
                items[i].GetChild(1).GetComponent<UIShiny>().enabled = true;
            }
        }
    }


    void LoadWeapons()
    {
        int ID = 1;
        for (int i = 0; i < WeaponDatabaseSO.weapons.Count; i++)
        {

            if (ID == 1)
                Database.UnlockedKnives(ID, 1);

            if(Database.UnlockedKnives(ID) == 1)
                WeaponDatabaseSO.weapons[i].state = WeaponState.Purchased;
            else
                WeaponDatabaseSO.weapons[i].state = WeaponState.Locked;

            if(Database.SelectedKnife == ID)
                WeaponDatabaseSO.weapons[i].state = WeaponState.Selected;

            ID++;
        }


        UpdateList();
    }

    // Update is called once per frame
  public void SelectKnife(int id)
    {
        if (Database.UnlockedKnives(id) == 1)
        {
            Database.SelectedKnife = id;
            LoadWeapons();
        }
        Debug.Log("Pressed with button ID: " + id);
    }
}
