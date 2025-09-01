using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControl : MonoBehaviour
{
    public FixedTouchField fixedTouchField;
    public CameraLook cameraLook;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cameraLook.LockInput = fixedTouchField.TouchDist;
    }
}
