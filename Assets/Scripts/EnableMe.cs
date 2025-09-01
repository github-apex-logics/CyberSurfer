using UnityEngine;

public class EnableMe : MonoBehaviour
{

    public float timeDelay;
    public GameObject target;


    private void OnEnable()
    {
        Invoke(nameof(Starts), timeDelay);
        
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Starts()
    {
        target.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
