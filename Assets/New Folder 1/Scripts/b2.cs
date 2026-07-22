using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float fuseTime = 3f;        // seconds before explosion
    public float explosionRadius = 5f; // how far it affects objects
    public float explosionForce = 500f;
    public int damage = 20;
    public GameObject explosionVFX;    // optional particle prefab

    private bool exploded = false;

    void Start()
    {
        // Automatically explode after fuse time
        Invoke(nameof(Explode), fuseTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Optional: explode early on impact
        if (!exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (exploded) return; // prevent double trigger
        exploded = true;

        // Spawn explosion effect
        if (explosionVFX != null)
        {
            GameObject fx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(fx, 3f); // remove effect after a few seconds
        }

        // Add explosion force
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            // Damage enemies
            EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }

        // ✅ Destroy bomb object after exploding
        Destroy(gameObject);
    }
}