using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public float rotationSpeed = 1f; // degrees per second

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
        // If using URP or want real-time update:
        DynamicGI.UpdateEnvironment();
    }
}
