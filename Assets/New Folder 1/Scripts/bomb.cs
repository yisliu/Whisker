using UnityEngine;
using System.Collections;

public class TEnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int startingHealth = 3;
    private int currentHealth;

    [Header("Death Settings")]
    [SerializeField] private int wseconds = 2;             // Wait time before destroy (integer)
    [SerializeField] private ParticleSystem particles;     // Assign particle system in Inspector
    [SerializeField] private GameObject rawModel;          // Alive model
    [SerializeField] private GameObject cookedModel;       // Cooked model
    [SerializeField] private UnityEngine.AI.NavMeshAgent robot; // Optional NavMeshAgent

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
        // Stop NavMeshAgent movement
        if (robot != null)
        {
            robot.isStopped = true;
        }

        // Play particles
        if (particles != null)
        {
            particles.Play();
        }

        // Swap models
        if (rawModel != null) rawModel.SetActive(false);
        if (cookedModel != null) cookedModel.SetActive(true);

        // Wait for the given number of seconds (cast int → float)
        yield return new WaitForSeconds(wseconds);

        // Destroy the entire enemy
        Destroy(gameObject);
    }
}