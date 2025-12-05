using UnityEngine;
using UnityEngine.Events;

public class PanelButtonAction : MonoBehaviour
{
    public PanelClose panel;
    public UnityEvent onAfterClose;


    private void Start()
    {
        panel = GetComponentInParent<PanelClose>();
        this.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ClosePanelWithEvent);
    }

    public void ClosePanelWithEvent()
    {
        panel.ClosePanel(() => onAfterClose?.Invoke());
    }
}
