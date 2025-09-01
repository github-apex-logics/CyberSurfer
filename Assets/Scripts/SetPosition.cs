using UnityEngine;

public class SetPosition : MonoBehaviour
{

    public float minX, maxX, conY, rightZ, leftZ;
    float posX, posY, posZ; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        posX = Random.Range(minX, maxX);
        posY = conY;
        posZ = Random.Range(0, 2) == 1 ? rightZ : leftZ;
        

       
        transform.position = new Vector3(posX, posY, posZ);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
