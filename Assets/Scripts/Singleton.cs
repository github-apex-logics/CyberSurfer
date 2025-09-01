using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(Instance);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
