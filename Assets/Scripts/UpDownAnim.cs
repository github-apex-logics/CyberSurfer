using System.Collections;

using UnityEngine;

public class UpDownAnim : MonoBehaviour
{
    Vector2 temp, origin;

    public float timeDelay, speed;

    public bool upDown;
    void OnEnable()
    {
        if (upDown)
            StartCoroutine(UpDown());
        else
            StartCoroutine(LeftRight());
    }

    public void Start()
    {
        origin = transform.localPosition;
        temp = transform.localPosition;

        
    }

    IEnumerator UpDown()
    {

        for (int i = 0; i < 10; i++)
        {
            temp.y += speed;
            transform.localPosition = temp;
            yield return new WaitForSecondsRealtime(timeDelay);

        }
        for (int i = 0; i < 10; i++)
        {
            temp.y -= speed;
            transform.localPosition = temp;
            yield return new WaitForSecondsRealtime(timeDelay);

        }

        transform.localPosition = origin;


        StartCoroutine(UpDown());
    }



    IEnumerator LeftRight()
    {
        temp = transform.localPosition;
        origin = transform.localPosition;


        for (int i = 0; i < 10; i++)
        {
            temp.x += speed;
            transform.localPosition = temp;
            yield return new WaitForSecondsRealtime(timeDelay);

        }
        for (int i = 0; i < 10; i++)
        {
            temp.x -= speed;
            transform.localPosition = temp;
            yield return new WaitForSecondsRealtime(timeDelay);

        }

        transform.localPosition = origin;


        StartCoroutine(LeftRight());
    }
}
