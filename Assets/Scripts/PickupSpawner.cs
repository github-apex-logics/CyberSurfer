using LightDI;
using System.Collections;

using UnityEngine;

public class PickupSpawner : MonoBehaviour, ISystem
{
 
    public float spawnInterval = 2f;
    public bool canSpawn = true;

    private void Awake()
    {
        InjectionManager.RegisterSystem(this);
        canSpawn = true;        
    }

    public void ResetSpawn()
    {
        StartCoroutine(SpawnLoop());
    }

  public IEnumerator SpawnLoop()
    {
        canSpawn = false;
        while (!canSpawn)
        {
            yield return new WaitForSeconds(timeInterval());
            canSpawn = true;
        }
    }


  public float timeInterval()
    {
        return Random.Range(spawnInterval, spawnInterval + 3);
    }
   
}
