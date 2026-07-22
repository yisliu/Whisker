using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class FlyingEnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float turnSpeed = 3f;
    public float hoverHeight = 5f; // height above ground
    public float hoverSwayAmplitude = 0.5f;
    public float hoverSwaySpeed = 2f;

    [Header("Chase Settings")]
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float damage = 10f;
    public float attackCooldown = 2f;

    private Transform player;
    private EnemyHealth enemyHealth;
    private float attackTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyHealth = GetComponent<EnemyHealth>();

        if (player == null)
        {
            Debug.LogWarning("[FlyingEnemyAI] Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player == null || enemyHealth == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= chaseRange)
            ChasePlayer(distance);
        else
            HoverIdle();
    }

    void ChasePlayer(float distance)
    {
        // Smoothly rotate toward the player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, turnSpeed * Time.deltaTime);

        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // Keep hovering
        ApplyHover();

        // Attack if close enough
        if (distance <= attackRange && attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }

        attackTimer -= Time.deltaTime;
    }

    void HoverIdle()
    {
        ApplyHover();
        // Optionally: slow patrol or rotate in place
        transform.Rotate(Vector3.up, 30f * Time.deltaTime);
    }

    void ApplyHover()
    {
        float hoverOffset = Mathf.Sin(Time.time * hoverSwaySpeed) * hoverSwayAmplitude;
        Vector3 targetPos = new Vector3(transform.position.x, hoverHeight + hoverOffset, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
    }

    void Attack()
    {
        Debug.Log("[FlyingEnemyAI] Attacking player!");

        var health = player.GetComponent<PlayerHealth>();
        if (health != null)
            health.TakeDamage((int)damage);
    }
}