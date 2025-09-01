using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private float xMove;
    private float yMove;
    private float xRotation;
    [SerializeField] private Transform PlayerBody;
    public Vector2 LockInput;
    public float Senstivity = 40f;
    // Update is called once per frame
    void Update()
    {
        xMove = LockInput.x * Senstivity * Time.deltaTime; 
        yMove = LockInput.y * Senstivity * Time.deltaTime;
        //xRotation -= yMove;
        xRotation = 25f;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        PlayerBody.Rotate(Vector3.up * xMove);
    }
}
