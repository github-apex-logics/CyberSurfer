using UnityEngine;
using System.Collections.Generic;

public class SpawningPlatform : MonoBehaviour
{
    public Transform player;
    public GameObject[] rampPrefabs;
    public RampPooler pooler;
    public float distanceAhead = 60f;
    public float cleanupDistanceBehind = 30f;

    private float nextSpawnZ = 0f;
    private List<(GameObject ramp, GameObject prefab, float startZ)> activeRamps = new();

   

    void Update()
    {
        // Spawn ramps ahead
        while (player.position.x + distanceAhead > nextSpawnZ)
        {
            SpawnNextRamp();
        }

        // Cleanup ramps behind player
        for (int i = activeRamps.Count - 1; i >= 0; i--)
        {
            if (player.position.x - activeRamps[i].startZ > cleanupDistanceBehind)
            {
                pooler.ReturnRamp(activeRamps[i].prefab, activeRamps[i].ramp);
                activeRamps.RemoveAt(i);
            }
        }
    }

    void SpawnNextRamp()
    {
        GameObject prefab = rampPrefabs[Random.Range(0, rampPrefabs.Length)];
        GameObject ramp = pooler.GetRamp(prefab);

        ramp.transform.position = new Vector3(nextSpawnZ,190,72);

       

        Ramp rampComponent = ramp.GetComponent<Ramp>();
        float rampLength = rampComponent != null ? rampComponent.length : 10f;

        activeRamps.Add((ramp, prefab, nextSpawnZ));
        nextSpawnZ += rampLength;
    }
}
