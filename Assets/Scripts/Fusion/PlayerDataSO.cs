using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "GameData/PlayerData")]
public class PlayerDataSO : ScriptableObject
{
    public  string[] playerNames = new string[4];
}