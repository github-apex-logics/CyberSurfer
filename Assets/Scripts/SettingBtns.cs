
using PlayFab.MultiplayerModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingBtns : MonoBehaviour
{

    public Image[] imgBtns;
    public Sprite fill, blank;
    public Color color;

    public void SwitchBtnVisuals(int a)
    {
        for (int i = 0; i < 3; i++)
        {
            if (i == a)
            {
                imgBtns[i].sprite = fill;
                imgBtns[i].GetComponentInChildren<TextMeshProUGUI>().color = color;// Color.magenta;
            }
            else 
            {
                imgBtns[i].sprite = blank;
                imgBtns[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            }
        }
    }


}





