using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDatabase", menuName = "Shop/Weapon Database")]
public class WeaponDatabaseSO : ScriptableObject
{
    [Header("All Weapons in Shop")]
    public List<WeaponSO> weapons = new List<WeaponSO>();

    //public void UnlockWeapon(int index)
    //{
    //    if (weapons[index].state == WeaponState.Locked)
    //        weapons[index].state = WeaponState.Unlocked;
    //}

    //public void PurchaseWeapon(int index)
    //{
    //    if (weapons[index].state == WeaponState.Unlocked)
    //        weapons[index].state = WeaponState.Purchased;
    //}

    //public void EquipWeapon(int index)
    //{
    //    // Unequip any currently equipped weapon
    //    foreach (var w in weapons)
    //    {
    //        if (w.state == WeaponState.Equipped)
    //            w.state = WeaponState.Purchased;
    //    }

    //    weapons[index].state = WeaponState.Equipped;
    //}
}




public enum WeaponState
{
    Locked,
    Purchased,
    Selected
}

[Serializable]
public class WeaponSO 
{
    [Header("Weapon Info")]
    public string weaponName;
    public int id;
    public Sprite icon;
    public int price;
    public Texture base_texture;
    public Texture emission_texture;

    [Header("Runtime State")]
    public WeaponState state = WeaponState.Locked;
}
