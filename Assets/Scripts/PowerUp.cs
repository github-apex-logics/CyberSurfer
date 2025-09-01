using LightDI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour, IInjectable
{
    [Range(1, 3)]
    public int numberOfPowersToChoose = 1;

    public List<Powers> selectedPowers = new List<Powers>();
    public GameObject boost, slash, homingSlash;

    [HideInInspector] public Powers powerUp;
    [Inject] private PickupSpawner spawner;



    private void OnEnable()
    {
        HandlePowerUpEnable();
    }


    private void Start()
    {
        StartCoroutine(InjectionDelay());
    }

    IEnumerator InjectionDelay()
    {
        yield return new WaitForEndOfFrame();
        InjectionManager.RegisterObject(this);
        HandlePowerUpEnable();
    }

    private void HandlePowerUpEnable()
    {
        if (spawner == null) return;

        var ps = GetComponent<ParticleSystem>();

        if (!spawner.canSpawn)
        {
            if (ps != null) ps.Stop();
            this.gameObject.SetActive(false);
            foreach (Transform t in transform)
                t.gameObject.SetActive(false);
        }
        else
        {
            if (ps != null) ps.Play();
            transform.GetChild(0).gameObject.SetActive(true);

            spawner.ResetSpawn();
            ChoosePowerUps();
        }
    }

    public void ChoosePowerUps()
    {
        if (selectedPowers == null || selectedPowers.Count == 0)
        {
            Debug.LogWarning("No powers defined in selectedPowers list.");
            return;
        }

        // Shuffle
        for (int i = 0; i < selectedPowers.Count; i++)
        {
            int rand = Random.Range(i, selectedPowers.Count);
            (selectedPowers[i], selectedPowers[rand]) = (selectedPowers[rand], selectedPowers[i]);
        }

        powerUp = selectedPowers[0];
        ChangeIcon(powerUp);
    }

    public void ChangeIcon(Powers p)
    {
        slash.SetActive(p == Powers.Slash);
        boost.SetActive(p == Powers.Boost);
        homingSlash.SetActive(p == Powers.HomingSlash);
    }

    public void PostInject()
    {
        // Not used, required by IInjectable
    }
}
