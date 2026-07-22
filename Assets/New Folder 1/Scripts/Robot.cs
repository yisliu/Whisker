using UnityEngine;
using UnityEngine.AI;


public class Robot : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform player;
    NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
		if(player!=null){
			agent.SetDestination(player.position);
		}
    }
}
