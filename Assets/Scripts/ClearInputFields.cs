
using TMPro;
using UnityEngine;

public class ClearInputFields : MonoBehaviour
{

    public TMP_InputField[] fields;

    // Start is called before the first frame update
    void OnEnable()
    {
       ClearFields();
    }

    public void ClearFields()
    {
        foreach (TMP_InputField field in fields)
        {
            field.text = "";
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
