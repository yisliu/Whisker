using UnityEngine;

public class FriedChickenPickup : MonoBehaviour
{
    public int healthRestore = 20;
    public ParticleSystem pickupEffect; // Optional sparkle or flash

    private bool isPickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;

        if (other.CompareTag("Player"))
        {
            isPickedUp = true;

            // Heal the player if they have a health script
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.RestoreHealth(healthRestore);
            }

            // Optional particle effect
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Remove the chicken object
            Destroy(gameObject);
        }
    }
}