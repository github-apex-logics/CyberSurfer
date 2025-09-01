using UnityEngine;

public class RotateTowards : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up; // Set axis: Vector3.right, Vector3.up, Vector3.forward
    public float rotationSpeed = 50f; // Degrees per second

    void Update()
    {
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
