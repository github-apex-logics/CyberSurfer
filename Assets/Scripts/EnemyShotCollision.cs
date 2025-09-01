
using UnityEngine;

public class EnemyShotCollision : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slash"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
