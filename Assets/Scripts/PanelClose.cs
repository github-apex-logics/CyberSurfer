using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelClose : MonoBehaviour
{
    public float delayTime;
    public PanelAnimation[] panelAnimations;
    public UnityEvent OnClose;
    private void Start()
    {
        panelAnimations = gameObject.GetComponentsInChildren<PanelAnimation>();
    }

    public void ClosePanel()
    {
        StartCoroutine(closeDelay());
        for (int i = 0; i < panelAnimations.Length; i++)
        {
            panelAnimations[i].ClosePanel();    
        }
    }

    IEnumerator closeDelay()
    {
        yield return new WaitForSecondsRealtime(delayTime);
        this.gameObject.SetActive(false);
        OnClose.Invoke();
    }

}
