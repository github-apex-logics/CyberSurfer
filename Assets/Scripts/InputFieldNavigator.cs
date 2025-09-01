using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InputFieldNavigator : MonoBehaviour
{
    public TMP_InputField nextInputField;

    private TMP_InputField currentField;

    private void Awake()
    {
        currentField = GetComponent<TMP_InputField>();
        currentField.onSubmit.AddListener(OnSubmit);
    }

    private void OnSubmit(string text)
    {
        // On mobile, Enter usually submits — this shifts focus
        if (nextInputField != null)
        {
            EventSystem.current.SetSelectedGameObject(nextInputField.gameObject);
            nextInputField.ActivateInputField();
        }
        else
        {
            // Optional: Deselect everything if it's the last field
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        // Support Tab key (only works if hardware keyboard is used)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OnSubmit(currentField.text);
        }
    }
}
