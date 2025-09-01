using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisable : MonoBehaviour
{

    public bool OnTime;
    public float time;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (OnTime)
        {
            Invoke(nameof(DisbaleMe), time);
        }
    }

    // Update is called once per frame
    public void DisbaleMe()
    {
        this.gameObject.SetActive(false);
    }
}
