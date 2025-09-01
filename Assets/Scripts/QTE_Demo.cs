
using UnityEngine;

public class QTE_Demo : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject[] options;
    public GameObject correct;


    // Start is called before the first frame update
    void Start()
    {
       
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }


    void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIndex = Random.Range(i, array.Length);
            (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
        }
    }


}
