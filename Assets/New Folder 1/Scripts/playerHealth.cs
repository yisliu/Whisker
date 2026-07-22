using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Prevent further damage after death

        currentHealth -= amount;
        ScoreManager.Instance?.SubtractPoints(ScoreManager.Instance.pointsLostOnHit);
        Debug.Log("Player took damage: " + amount + " | Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 🧠 Add this method so pickups (like cooked chicken) can heal the player
    public void RestoreHealth(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("🍗 Player healed: +" + amount + " | Current Health: " + currentHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("💀 Player Died!");

        // Example: disable player movement
        var controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        // Example: hide player
        gameObject.SetActive(false);

        // TODO: Trigger respawn, reload scene, or show Game Over UI
    }
}	