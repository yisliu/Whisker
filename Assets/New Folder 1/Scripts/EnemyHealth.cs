using UnityEngine;
using System.Collections;

public class T2EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private EnemyStatsSO stats;

    [Header("Health Settings")]
    [SerializeField] private int startingHealth = 3;   // used only when stats is not assigned
    private int currentHealth;

    [Header("Death Settings")]
    [SerializeField] private float wseconds = 2f;      // wait time before destroying
    [SerializeField] private ParticleSystem particles; // assign your particle system
    [SerializeField] private GameObject rawModel;      // assign your "raw" model in Inspector
    [SerializeField] private GameObject cookedModel;   // assign your "cooked" model in Inspector
    [SerializeField] private UnityEngine.AI.NavMeshAgent robot; // optional NavMeshAgent

    [SerializeField] private ParticleSystem smokeEffect;   // Smoke effect when cooked model disappears

    void Awake()
    {
        currentHealth = stats != null ? stats.maxHealth : startingHealth;
        if (rawModel != null) rawModel.SetActive(true);
        if (cookedModel != null) cookedModel.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        // Stop enemy movement if it has a NavMeshAgent
        if (robot != null)
        {
            robot.isStopped = true;
        }

        // Play particles if assigned
        if (particles != null)
        {
            particles.Play();
        }

        // Swap models
        if (rawModel != null) rawModel.SetActive(false);
        if (cookedModel != null) cookedModel.SetActive(true);

        // Wait before removing / disabling the enemy
        yield return new WaitForSeconds(wseconds);

        // Play smoke effect before destroying cooked model
        PlaySmokeEffect();

        // (Optional) Destroy enemy after effect finishes
        Destroy(gameObject);
    }

    private void PlaySmokeEffect()
    {
        if (smokeEffect != null)
        {
            // Instantiate smoke effect at cooked model position
            ParticleSystem smoke = Instantiate(smokeEffect, transform.position, Quaternion.identity);
            smoke.Play();
            Destroy(smoke.gameObject, smoke.main.duration + smoke.main.startLifetime.constantMax);
        }
    }
}
