using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;

    [Header("Death Settings")]
    [SerializeField] private int wseconds = 2;             // Wait time before destroy
    [SerializeField] private ParticleSystem particles;     // Death particle effect
    [SerializeField] private GameObject rawModel;          // Alive model
    [SerializeField] private GameObject cookedModel;       // Cooked model (pickup)
    [SerializeField] private UnityEngine.AI.NavMeshAgent robot; // Optional NavMeshAgent

    [Header("Pickup Settings")]
    [SerializeField] private int healAmount = 20;          // How much health player restores
    [SerializeField] private ParticleSystem pickupEffect;  // Optional effect when picked up
    [SerializeField] private ParticleSystem smokeEffect;   // Smoke effect when cooked model disappears
    private bool canBePickedUp = false;

    void Awake()
    {
        currentHealth = startingHealth;
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
        ScoreManager.Instance?.AddPoints(ScoreManager.Instance.pointsPerKill);
        WinScreenManager.Instance?.EnemyKilled();

        // Stop NavMeshAgent
        if (robot != null)
        {
            robot.isStopped = true;
        }

        // Play death particles
        if (particles != null)
        {
            particles.Play();
        }

        // Swap to cooked model
        if (rawModel != null) rawModel.SetActive(false);
        if (cookedModel != null) cookedModel.SetActive(true);

        // Allow pickup
        canBePickedUp = true;

        // Wait for wseconds before auto-destroy (if not picked up)
        yield return new WaitForSeconds(wseconds);

        if (canBePickedUp) // Only destroy if not picked up
        {
            PlaySmokeEffect();
            Destroy(gameObject);
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if (!canBePickedUp) return; // Only after it's cooked
        if (!other.CompareTag("Player")) return;

        PlayerHealth hp = other.GetComponent<PlayerHealth>();
        if (hp != null)
        {
            hp.RestoreHealth(healAmount);
        }

        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        PlaySmokeEffect(); // Play smoke when cooked model disappears

        canBePickedUp = false;
        Destroy(gameObject); // Remove after being eaten
    }
}