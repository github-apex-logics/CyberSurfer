using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnEffector : MonoBehaviour
{
    public Image[] Btns;

    [Tooltip("Panels Should be in the exact same order of there corresponding buttons")]
    public GameObject[] panels;

    public bool includePanels;

    public TMP_Text titleTxt;

    public Color white, pink;



    private void Start()
    {
        for (int i = 0; i < Btns.Length; i++)
        {
            int index = i; // capture the correct value
            if (!includePanels)
            { 
                Btns[i].GetComponent<Button>().onClick.AddListener(() => OnBtnClick(index));
            }
            else 
            { 
                Btns[i].GetComponent<Button>().onClick.AddListener(() => OnBtnClick_Panel(index));
            }
        }

        if (!includePanels)
        {
            OnBtnClick(0);
        }
        else
        {
            OnBtnClick_Panel(0);
        }

    }



    public void OnBtnClick_Panel(int num)
    {
        for (int i = 0; i < Btns.Length; i++)
        {

            if (i == num)
            {
                Btns[i].color = pink;
                Btns[i].GetComponentInChildren<TMP_Text>().color = white;
                panels[i].SetActive(true);
            }
            else
            {


                Btns[i].color = white;
                Btns[i].GetComponentInChildren<TMP_Text>().color = pink;
                panels[i].SetActive(false);
            }

        }
    }



    public void OnBtnClick(int num)
    {
        for (int i = 0; i < Btns.Length; i++)
        {
           
            if (i == num)
            {
                Btns[i].color = pink;
                Btns[i].GetComponentInChildren<TMP_Text>().color = white;
                titleTxt.text = Btns[i].GetComponentInChildren<TMP_Text>().text;
            }
            else
            {


                Btns[i].color = white;
                Btns[i].GetComponentInChildren<TMP_Text>().color = pink;
            }

        }
    }
}
