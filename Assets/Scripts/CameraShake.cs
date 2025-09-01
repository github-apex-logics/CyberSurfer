using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    public float duration, magnitude;


    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
