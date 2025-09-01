using System.Collections;
using UnityEngine;

public class OrbSizeanim : MonoBehaviour
{

    Vector3 temp, original;

    private void OnEnable()
    {
        original = transform.localScale;
        StartCoroutine(StartSize());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private IEnumerator StartSize()
    {
        
        temp = transform.localScale;
        for (int i = 0; i < 40; i++)
        {
            temp.x += 0.25f;
            temp.y += 0.25f;
            temp.z += 0.25f;

            transform.localScale = temp;
            yield return new WaitForSeconds(0.01f);
            
        }
        

    }

    private void OnDisable()
    {
        transform.localScale = original;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
