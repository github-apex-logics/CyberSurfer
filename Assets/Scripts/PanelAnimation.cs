using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAnimation : MonoBehaviour
{
    public bool isSpawn;
    public bool isHorizontal;
    public bool isFadeIn;
    
    float smoothness;
    float delayTime;

    private void Init()
    {
        smoothness = 0.1f;
        delayTime = 0.005f;
    }

    void OnEnable()
    {
        Init();
        if (isSpawn)
            StartCoroutine(StartEffect());
        if (isFadeIn)
            StartCoroutine(FadeIN());
    }


    IEnumerator StartEffect()
    {
        this.transform.localScale = new Vector3(0, 0, 0);
        float a = 0;
        if (isHorizontal)
        {
            for (int i = 0; i <= 10; i++)
            {
                this.transform.localScale = new Vector3(a, 1, 1);
                yield return new WaitForSecondsRealtime(delayTime);
                a += smoothness;
            }
        }
        else
        {
            for (int i = 0; i <= 10; i++)
            {
                this.transform.localScale = new Vector3(1, a, 1);
                yield return new WaitForSecondsRealtime(delayTime);
                a += smoothness;
            }
        }
       
        this.transform.localScale = new Vector3(1, 1, 1);
    }


    IEnumerator FadeIN()
    {

        Image img = GetComponent<Image>();
       // img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        float a = 0;
        for (int i = 0; i < 10; i++)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return new WaitForSecondsRealtime(delayTime);
            a += smoothness;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
    }


    IEnumerator FadeOut()
    {

        Image img = GetComponent<Image>();
        // img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        float a = 1;
        for (int i = 0; i < 10; i++)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);
            yield return new WaitForSecondsRealtime(delayTime);
            a -= smoothness;
        }
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
    }
    IEnumerator EndEffect()
    {
        this.transform.localScale = new Vector3(0, 0, 0);
        float a = 1;
        if (isHorizontal)
        {
            for (int i = 0; i <= 10; i++)
            {
                this.transform.localScale = new Vector3(a, 1, 1);
                yield return new WaitForSecondsRealtime(delayTime);
                a -= smoothness;
            }
        }
        else
        {
            for (int i = 0; i <= 10; i++)
            {
                this.transform.localScale = new Vector3(1, a, 1);
                yield return new WaitForSecondsRealtime(delayTime);
                a -= smoothness;
            }
        }

        //this.transform.localScale = new Vector3(, 1, 1);
    }




    public void ClosePanel()
    {
        if (isSpawn)
            StartCoroutine(EndEffect());
        if (isFadeIn)
            StartCoroutine(FadeOut());
    }


}
