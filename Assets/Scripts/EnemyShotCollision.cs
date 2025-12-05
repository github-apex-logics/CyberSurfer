
using UnityEngine;

public class EnemyShotCollision : MonoBehaviour
{

    public GameObject hitEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Slash"))
        {
            SpawnHitEffect();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    void SpawnHitEffect()
    {
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
    }
}
