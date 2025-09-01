using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForceScrollTop : MonoBehaviour
{
    private ScrollRect scrollRect;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        StartCoroutine(ResetScrollWhenReady());
    }

    private IEnumerator ResetScrollWhenReady()
    {
        // Wait for multiple frames to let GridLayout & ContentSizeFitter rebuild
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Canvas.ForceUpdateCanvases(); // force layout calculation
        scrollRect.normalizedPosition = new Vector2(0, 1); // top
        Canvas.ForceUpdateCanvases();
    }
}
