using UnityEngine;
using UnityEngine.AI;

public class BirdAI : MonoBehaviour
{
    public Transform player;
    public float chaseDistance = 20f;
    public float avoidHeight = 1.5f;
    public float speed = 200f;
    public float evadeSpeed = 225f;
    public float minEvadeDistance = 3f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > chaseDistance) return;

        Vector3 birdPos = transform.position;
        Vector3 playerPos = player.position;

        // Keep bird Y level for NavMesh movement
        playerPos.y = birdPos.y;

        if (player.position.y > avoidHeight)
        {
            // Evade horizontally
            Vector3 dirAway = (birdPos - playerPos).normalized;
            Vector3 evadeTarget = birdPos + dirAway * minEvadeDistance;
            agent.speed = evadeSpeed;
            agent.SetDestination(evadeTarget);
        }
        else
        {
            // Chase
            agent.speed = speed;
            agent.SetDestination(playerPos);
        }
    }
}