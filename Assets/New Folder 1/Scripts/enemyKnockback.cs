using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 explosionPos, float force, float radius)
    {
        if (rb == null) return;
        StartCoroutine(KnockbackRoutine(explosionPos, force, radius));
    }

    IEnumerator KnockbackRoutine(Vector3 explosionPos, float force, float radius)
    {
        // temporarily disable NavMeshAgent control
        if (agent != null) agent.enabled = false;

        // enable physics
        rb.isKinematic = false;

        // apply explosion force
        rb.AddExplosionForce(force, explosionPos, radius, 0.5f, ForceMode.Impulse);

        // wait briefly to let the push happen
        yield return new WaitForSeconds(0.4f);

        // stop rigidbody movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // freeze physics again
        rb.isKinematic = true;

        // re-enable agent control
        if (agent != null) agent.enabled = true;
    }
}