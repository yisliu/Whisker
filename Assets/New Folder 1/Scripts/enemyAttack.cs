using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;           // Assign in Inspector
    public float attackRange = 25f;
    public float damage = 10f;
    public float attackCooldown = 2f;
    public float evadeDistance = 20f;

    private NavMeshAgent agent;
    private float lastAttackTime;
    private int attackPattern; // 0 = chase, 1 = strafe, 2 = lunge

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        PickNewPattern();
    }

    void Update()
{
    if (player == null) return;

    // 1. Ensure the agent is active, enabled, and actually snapped to a NavMesh
    if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
    {
        return; // Skip this frame until the agent is ready
    }

    PlayerController2 playerController = player.GetComponent<PlayerController2>();
    bool playerInAir = playerController != null && playerController.isJumping;

    float distance = Vector3.Distance(transform.position, player.position);

    if (playerInAir)
    {
        // Evade the player
        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 evadeTarget = transform.position + dirAway * evadeDistance;
        agent.SetDestination(evadeTarget);
    }
    else
    {
        // Normal attack patterns
        switch (attackPattern)
        {
            case 0: // Normal chase
                agent.SetDestination(player.position);
                break;

            case 1: // Strafe around the player
                Vector3 dir = (transform.position - player.position).normalized;
                Vector3 strafe = Quaternion.Euler(0, 90, 0) * dir;
                Vector3 target = player.position + strafe * 3f;
                agent.SetDestination(target);
                break;

            case 2: // Lunge attack
                if (distance < attackRange * 2f && Time.time > lastAttackTime + attackCooldown)
                {
                    agent.speed *= 2; // temporary speed boost
                    agent.SetDestination(player.position);
                }
                break;
        }

        // Damage if in range
        if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(Mathf.RoundToInt(damage));
            }
            lastAttackTime = Time.time;
            PickNewPattern(); // switch it up
        }
    }
}

    void PickNewPattern()
    {
        attackPattern = Random.Range(0, 3); // 0, 1, or 2
        agent.speed = 200f; // reset speed to normal
    }
}