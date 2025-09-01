using UnityEngine;
using System.Collections.Generic;

public class RampPooler : MonoBehaviour
{
    public GameObject[] rampPrefabs;
    public int poolSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> rampPools = new();

    void Awake()
    {
        foreach (var prefab in rampPrefabs)
        {
            Queue<GameObject> pool = new();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(prefab);

                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            rampPools[prefab] = pool;
        }
    }

    public GameObject GetRamp(GameObject prefab)
    {
        if (!rampPools.ContainsKey(prefab)) return null;

        Queue<GameObject> pool = rampPools[prefab];
        GameObject obj = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        obj.SetActive(true);
        return obj;
    }

    public void ReturnRamp(GameObject prefab, GameObject ramp)
    {
        ramp.SetActive(false);
        rampPools[prefab].Enqueue(ramp);
    }
}
