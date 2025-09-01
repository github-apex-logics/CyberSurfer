using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorMsg : MonoBehaviour
{
    public TMP_Text errorText;
    
   public void SetErrorMsg(string msg)
    {
        errorText.text = msg;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
