using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{

    public bool OnTime;
    public float time;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (OnTime)
        {
            Invoke(nameof(DestroyMe), time);
        }
    }

    // Update is called once per frame
    public void DestroyMe()
    {
        Destroy(this.gameObject);

    }
}
