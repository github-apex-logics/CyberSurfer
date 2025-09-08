using Fusion;
using System.Collections;
using UnityEngine;

public class Slash : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;
    bool s = false;
    [SerializeField] private ParticleSystem impactEffect;

    private TickTimer lifeTimer;

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
        StartCoroutine(Delaycollision());
    }

    IEnumerator Delaycollision()
    {
        yield return new WaitForSeconds(1f);
        s=true;
    }
    public void Launch(Vector3 direction)
    {
        transform.forward = direction;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            transform.position += transform.forward * speed * Runner.DeltaTime;

            if (lifeTimer.Expired(Runner))
                Runner.Despawn(Object);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Object.HasStateAuthority) return;
        else
        {

            if (other.TryGetComponent<CyberNetworkController>(out var player) && s)
            {
                // Example: apply damage logic
                Debug.Log("Slash hit player: " + player.Object.Id);
              //  player.ResetPos(); // if you have such method
               // if (player.HasInputAuthority)
                    player.RPC_RequestReset(Object.InputAuthority);
                Runner.Despawn(Object);
            }

            if (impactEffect != null)
                impactEffect.Play();

        }
    }
}
