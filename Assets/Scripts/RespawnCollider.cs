using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCollider : MonoBehaviour
{
    public Transform spawnPoint;


    private void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }


}
