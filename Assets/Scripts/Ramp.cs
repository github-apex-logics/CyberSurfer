using UnityEngine;


public class Ramp : MonoBehaviour
{
    public float length;// = 10f; // Manually assign in prefab

    float zLength;
    public GameObject deadZone;


    private void Awake()
    {
        deadZone = this.transform.GetChild(transform.childCount-1).gameObject;
        length = deadZone.GetComponent<MeshRenderer>().bounds.size.x;

    }

}
