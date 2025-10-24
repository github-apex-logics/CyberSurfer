using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    #region === References ===
    [Header("References")]
    [Tooltip("List of all player transforms in the scene.")]
    public Transform[] players;

    [Tooltip("Projectile spawn position.")]
    public Transform firePoint;

    [Tooltip("Projectile prefab.")]
    public GameObject projectilePrefab;

    [Tooltip("Rotating part of the tower.")]
    public Transform turretHead;


    public AudioSource GunFireSound;
    #endregion

    #region === Combat Settings ===
    [Header("Combat Settings")]
    public float detectionRadius = 30f;
    public float shootInterval = 2f;
    public float projectileSpeed = 15f;
    #endregion

    #region === Rotation Settings ===
    [Header("Rotation Settings")]
    public float lookSpeed = 5f;
    #endregion

    #region === Tower Types ===
    [Header("Tower 1 Settings")]
    public bool tower1;
    public GameObject T1_Gun;
    public ParticleSystem T1_GunEmitter;
   

    [Header("Tower 2 Settings")]
    public bool tower2;
    public GameObject T2_Gun;

    [Header("Tower 3 Settings")]
    public bool tower3;

    [Header("Tower 4 Settings")]
    public bool tower4;
    #endregion

    #region === Private Variables ===
    private Transform targetPlayer;
    private float shootTimer;
    private bool toggleState;
    #endregion

    #region === Unity Methods ===
    private void Update()
    {
        if (players == null || players.Length == 0)
            return;

        targetPlayer = GetClosestOrRandomPlayer();
        if (targetPlayer == null)
            return;

        RotateTowardsTarget(targetPlayer);

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            Shoot(targetPlayer);
            shootTimer = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    #endregion

    #region === Target Acquisition ===
    private Transform GetClosestOrRandomPlayer()
    {
        List<Transform> playersInRange = new List<Transform>();
        float closestDistance = Mathf.Infinity;

        // Find players within detection radius
        foreach (Transform player in players)
        {
            if (player == null) continue;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= detectionRadius)
            {
                playersInRange.Add(player);
                closestDistance = Mathf.Min(closestDistance, distance);
            }
        }

        if (playersInRange.Count == 0)
            return null;

        // Get players with nearly equal distance (within 1 meter)
        List<Transform> equallyClose = playersInRange.FindAll(p =>
            Mathf.Abs(Vector3.Distance(transform.position, p.position) - closestDistance) < 1f);

        return equallyClose.Count > 0
            ? equallyClose[Random.Range(0, equallyClose.Count)]
            : null;
    }
    #endregion

    #region === Rotation ===
    private void RotateTowardsTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        if (tower1)
            direction.y -= 0.3f; // slightly tilt down
        else if (tower2)
            direction.y = 0f;    // flat rotation

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        turretHead.rotation = Quaternion.Slerp(
            turretHead.rotation, targetRotation, Time.deltaTime * lookSpeed);
    }
    #endregion

    #region === Shooting ===
    private void Shoot(Transform target)
    {
        if (projectilePrefab == null || firePoint == null || target == null)
            return;

        if (tower1)
        {
            FireProjectile(target);
            StartCoroutine(TowerOneEffect());
        }
        else if (tower2)
        {
            // TODO: Add Tower 2 shooting behavior here
        }
        else if (tower4)
        {
            // TODO: Add Tower 4 shooting behavior here
        }
    }

    private void FireProjectile(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir = (target.position - firePoint.position).normalized;
            rb.linearVelocity = dir * projectileSpeed;
        }

        Destroy(projectile, 5f); // Cleanup after delay
    }
    #endregion

    #region === Tower Effects ===
    private IEnumerator TowerOneEffect()
    {
        if (T1_Gun == null || T1_GunEmitter == null || GunFireSound == null)
            yield break;

        Vector3 originalPos = T1_Gun.transform.localPosition;
        Vector3 recoilPos = originalPos - new Vector3(0, 0, 0.5f);

        // Play fire effects
        GunFireSound.Play();
        T1_GunEmitter.gameObject.SetActive(true);
        T1_Gun.transform.localPosition = recoilPos;

        // Recoil return animation
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.01f);
            T1_Gun.transform.localPosition = Vector3.Lerp(
                T1_Gun.transform.localPosition, originalPos, 0.3f);
        }

        // Add gun rotation shake
        Quaternion originalRot = T1_Gun.transform.localRotation;
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            T1_Gun.transform.localRotation = originalRot * Quaternion.Euler(0, 0, 20f);
        }

        // Reset rotation
        T1_Gun.transform.localRotation = originalRot;
    }
    #endregion

    #region === Tower Utility Actions ===
    public void ToggleTower2()
    {
        toggleState = !toggleState;
        if (projectilePrefab)
        {
            StartCoroutine(DelayOff(toggleState));
            
        }
    }
    IEnumerator DelayOff(bool b)
    {
        if (b)
        {
            projectilePrefab.GetComponent<Animator>().ResetTrigger("LaserOff");
            projectilePrefab.SetActive(true);
        }
        else
        { 
            projectilePrefab.GetComponent<Animator>().SetTrigger("LaserOff");
            yield return new WaitForSeconds(0.49f);
            projectilePrefab.SetActive(false);
        }
        
    }




    public void ToggleTower4()
    {
        toggleState = !toggleState;

        foreach (Transform child in projectilePrefab.transform)
        {
            ParticleSystem ps = child.GetComponent<ParticleSystem>();
            if (ps == null) continue;

            if (toggleState)
            {
                ps.Play();
                GunFireSound.Play();
            }
            else
            {
                ps.Stop();
                GunFireSound.Stop();
            }
        }

        Debug.Log($"Tower4 State: {toggleState}");
    }


    #endregion
}
