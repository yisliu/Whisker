using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyAI2 : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] private EnemyStatsSO stats;

    public int damage = 10;             // used only when stats is not assigned
    public float attackCooldown = 1f;   // used only when stats is not assigned
    private float lastAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (stats != null)
            agent.speed = stats.moveSpeed;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamage(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryDamage(collision.gameObject);
        }
    }

    private void TryDamage(GameObject playerObj)
    {
        float cd  = stats != null ? stats.attackCooldown : attackCooldown;
        int   dmg = stats != null ? stats.attackDamage   : damage;

        if (Time.time - lastAttackTime >= cd)
        {
            PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(dmg);
                Debug.Log("Enemy hit player for " + dmg);
            }
            lastAttackTime = Time.time;
        }
    }
}