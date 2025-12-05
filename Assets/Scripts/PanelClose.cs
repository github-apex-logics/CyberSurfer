using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PanelClose : MonoBehaviour
{
    public float delayTime;
    public PanelAnimation[] panelAnimations;
    public UnityEvent OnClose;

    private Action onClosedCallback; // store specific action from the button

    private void Start()
    {
        panelAnimations = GetComponentsInChildren<PanelAnimation>();
    }

    // Default close (uses UnityEvent only)
    public void ClosePanel()
    {
        ClosePanel(null);
    }

    // Overload that accepts a specific action
    public void ClosePanel(Action onClosed)
    {
        onClosedCallback = onClosed;
        StartCoroutine(CloseDelay());

        foreach (var anim in panelAnimations)
            anim.ClosePanel();
    }

    public void ClosePanels(UnityEvent onClosed)
    {
       // onClosedCallback = onClosed;
        StartCoroutine(CloseDelay());

        foreach (var anim in panelAnimations)
            anim.ClosePanel();
    }

    private IEnumerator CloseDelay()
    {
        yield return new WaitForSecondsRealtime(delayTime);

        gameObject.SetActive(false);
        OnClose?.Invoke();

        // Execute specific callback if assigned
        onClosedCallback?.Invoke();
        onClosedCallback = null;
    }
}
