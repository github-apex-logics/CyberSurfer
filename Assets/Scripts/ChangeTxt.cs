using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeTxt : MonoBehaviour
{

    public TMP_Text txt;
    // Start is called before the first frame update
    void Start()
    {
       
        Invoke(nameof(Updates), 2f);
    }
    private void OnEnable()
    {
        txt.text = "Authenticating";
    }

    // Update is called once per frame
    void Updates()
    {
        txt.text = "Connecting";
    }
}
