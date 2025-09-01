using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    #region === References ===
    [Header("References")]
    public LevelManager LevelManager;
    public Transform player;
    public Transform boss;
    public Transform neck;
    public Transform firePoint, clapPoint;
    public Transform[] handPoints;
    public GameObject projectilePrefab, clapPrefab, handProjectile;
    public GameObject fireParticle, smokeParticle;
    public GameObject Laser1, Laser2, laserSound;
    public GameObject IncomingWarning;

    public Renderer targetRenderer;
    public Image healthBar;
    public Animator animator;
    #endregion

    #region === Combat Settings ===
    [Header("Combat")]
    public float detectionRadius = 10f;
    public float attackCooldown = 2f;
    public float projectileSpeed = 10f;
    public string[] attackTriggers;
    #endregion

    #region === Health ===
    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;
    #endregion

    #region === Emission Settings ===
    [Header("Emission")]
    public Color chargeColor = Color.red;
    public Color idleColor = Color.cyan;
    public float chargeTime = 1f;
    public float releaseTime = 0.5f;
    private Material[] materials;
    private Coroutine emissionCoroutine;
    #endregion

    #region === Internal State ===
    private float lastAttackTime;
    private bool wasInterrupted;
    private string randomTrigger;
    public float distanceFromPlayer = 100f;
    private bool canAttack;
    #endregion

    #region === Unity Events ===

    private void Start()
    {
        currentHealth = maxHealth;
        StartEmit();
        StartCoroutine(DelayAttack());
    }

    IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        LookAtPlayer(transform);
        UpdatePos();
        UpdateHealthUI();

        if (distance <= detectionRadius && canAttack && LevelManager.startGame)
        {
            TryAttack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Slash"))
        {
            TakeDamage(10);
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion

    #region === Boss Position ===

    void UpdatePos()
    {
        Vector3 bossPos = boss.position;
        bossPos.x = player.position.x + distanceFromPlayer;
        boss.position = bossPos;
    }

    #endregion

    #region === AI Logic ===

    private void LookAtPlayer(Transform t)
    {
        Vector3 dir = (player.position - t.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            t.rotation = Quaternion.Slerp(t.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void TryAttack()
    {
        if (wasInterrupted)
        {
            lastAttackTime = Time.time;
            wasInterrupted = false;
            return;
        }

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            IncomingWarning.SetActive(true);
            randomTrigger = attackTriggers[Random.Range(0, attackTriggers.Length)];
            animator.SetTrigger(randomTrigger);
            lastAttackTime = Time.time;
        }
    }

    public void ResetTrigger()
    {
        smokeParticle.SetActive(false);
        animator.ResetTrigger(randomTrigger);
    }

    #endregion

    #region === Attacks ===

    public void FireProjectile()
    {
        if (firePoint == null || projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null && player != null)
        {
            rb.linearVelocity = (player.position - firePoint.position).normalized * projectileSpeed;
        }
        Destroy(projectile, 3f);
    }

    public void ClapProjectile()
    {
        if (clapPoint == null || clapPrefab == null) return;

        GameObject projectile = Instantiate(clapPrefab, clapPoint.position, clapPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null && player != null)
        {
            rb.linearVelocity = (player.position - clapPoint.position).normalized * projectileSpeed;
        }

        SoundManager.instance.PlayClip(ClipName.BossClap);
        Destroy(projectile, 3f);
    }

    public void HandProjectile()
    {
        if (clapPoint == null || clapPrefab == null) return;

        StartCoroutine(FireAfterDelay());
        SoundManager.instance.PlayClip(ClipName.BossClap);
    }

    private IEnumerator FireAfterDelay()
    {
        float delayBeforeFire = 0.15f;
        List<Rigidbody> spawnedProjectiles = new List<Rigidbody>();

        for (int i = 0; i < 3; i++)
        {
            GameObject projectile = Instantiate(handProjectile, handPoints[i].position, handPoints[i].rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true;
                spawnedProjectiles.Add(rb);
                Destroy(projectile, 3f);
            }
        }

        yield return new WaitForSeconds(delayBeforeFire);

        foreach (Transform hand in handPoints)
        {
            hand.GetChild(0).gameObject.SetActive(false);
        }

        laserSound.SetActive(false);

        foreach (Rigidbody rb in spawnedProjectiles)
        {
            if (rb != null && player != null)
            {
                rb.isKinematic = false;
                if (rb != null && player != null)
                {
                    rb.linearVelocity = (player.position - rb.position).normalized * (projectileSpeed - 200);
                }

            }
        }
    }

    #endregion

    #region === Health System ===

    public void TakeDamage(int damage)
    {
        wasInterrupted = true;
        currentHealth -= damage;

        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        SoundManager.instance.PlayClip(ClipName.EnemyDie);
        animator.SetTrigger("Die");
        Destroy(gameObject, 4f);
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(2f);
       
        LevelManager.GameComplete();
    }

    #endregion

    #region === Particle and Laser Effects ===

    public void FireParticle(int i)
    {
        if (i == 1)
        {
            fireParticle.SetActive(true);
        }
        else
        {
            fireParticle.SetActive(false);
            smokeParticle.SetActive(true);
            SoundManager.instance.PlayClip(ClipName.BossGrunt);
        }
    }

    public void LaserParticle(int i)
    {
        if (i == 1)
        {
            laserSound.SetActive(true);
            Laser1.GetComponent<LineRenderer>().enabled = false;
            Laser2.GetComponent<LineRenderer>().enabled = false;
            Laser1.SetActive(true);
            Laser2.SetActive(true);
            SoundManager.instance.PlayClip(ClipName.BossGrunt);
        }
        else if (i == 2)
        {
            Laser1.GetComponent<LineRenderer>().enabled = true;
            Laser2.GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            Laser1.GetComponent<LineRenderer>().enabled = false;
            Laser2.GetComponent<LineRenderer>().enabled = false;
            Laser1.SetActive(false);
            Laser2.SetActive(false);
            laserSound.SetActive(false);
        }
    }

    #endregion

    #region === Emission Color Effects ===

    void StartEmit()
    {
        materials = targetRenderer.materials;

        foreach (var mat in materials)
        {
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", idleColor);
            }
        }
    }

    public void PlayEmissionEffect()
    {
        if (emissionCoroutine != null)
            StopCoroutine(emissionCoroutine);

        foreach (Transform hand in handPoints)
        {
            hand.GetChild(0).gameObject.SetActive(true);
        }

        laserSound.SetActive(true);
        emissionCoroutine = StartCoroutine(EmissionEffectRoutine());
    }

    private IEnumerator EmissionEffectRoutine()
    {
        float t = 0f;

        while (t < chargeTime)
        {
            t += Time.deltaTime;
            Color current = Color.Lerp(idleColor, chargeColor, t / chargeTime);
            foreach (var mat in materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", current);
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        t = 0f;
        while (t < releaseTime)
        {
            t += Time.deltaTime;
            Color current = Color.Lerp(chargeColor, idleColor, t / releaseTime);
            foreach (var mat in materials)
            {
                if (mat.HasProperty("_EmissionColor"))
                    mat.SetColor("_EmissionColor", current);
            }
            yield return null;
        }

        foreach (var mat in materials)
        {
            if (mat.HasProperty("_EmissionColor"))
                mat.SetColor("_EmissionColor", idleColor);
        }
    }

    #endregion
}
