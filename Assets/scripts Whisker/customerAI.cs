
using UnityEngine;
using UnityEngine.AI;

public class customerAI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Points")] 
    public Transform sPoint;

    public Transform ePoint;
    [Header("Timer")] 
    public float timer = 30f;

    [Header("Effects")] 
    public ParticleSystem happyP;

    public ParticleSystem sadP;
    private NavMeshAgent agent;
    private float wTimer;
    private enum State {Walking, Waiting, Leaving}
    private State state = State.Walking;
    
    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("Dialogue")] 
    [SerializeField] private string[] greetingLines;
    [SerializeField] private string[] thankYouLines;

    [SerializeField] private string[] badLines;
    //customerDialogue.Instance.StartDialogue(greetingLines);
    
    
    
    
    void Start()
    {
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }
        agent = GetComponent<NavMeshAgent>();
        wTimer = timer;

        if (sPoint != null)
        {
            agent.SetDestination(sPoint.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            return;
        }

        switch (state)
        {
            case State.Walking:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    agent.isStopped = true;
                    state = State.Waiting;
                    customerDialogue.Instance.StartDialogue(greetingLines);
                }

                break;
            case State.Waiting:
                wTimer -= Time.deltaTime;
                if (wTimer <= 0f)
                {
                    PlayEffect(sadP);
                    StartLeaving();
                }

                break;
            case State.Leaving:
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    Destroy(gameObject);
                }

                break;
        }
        
        UpdateAnimation();
        if (state == State.Waiting && Input.GetKeyDown(KeyCode.F))
        {
            customerDialogue.Instance.StartDialogue(greetingLines);
        }
    }

    public void CompleteOrder()
    {
        if (state != State.Waiting)
        {
            return;
        }
        customerDialogue.Instance.StartDialogue(thankYouLines);
        PlayEffect(happyP);
        StartLeaving();
    }

    /*void OnMouseDown()
    {
        if (state == State.Waiting)
        {
            customerDialogue.Instance.StartDialogue(greetingLines);
        }
    }*/

    private void StartLeaving()
    {
        state = State.Leaving;
        agent.isStopped = false;
        if (ePoint != null)
        {
            agent.SetDestination(ePoint.position);
        }
    }

    private void PlayEffect(ParticleSystem ps)
    {
        if (ps != null)
        {
            ps.Play();
        }
    }

    void UpdateAnimation()
    {
        if (anim == null)
        {
            return;
        }

        Vector3 ve = agent.velocity;
        if (ve.magnitude > 0.1f)
        {
            anim.SetBool("isBack", ve.z > 0f);
        }

        anim.speed = (state == State.Waiting) ? 0f : 1f;
    }

    public void WrongOrder()
    {
        if (state != State.Waiting)
        {
            return;
        }

        ScoreManager.Instance?.AddPoints(-10000);
        customerDialogue.Instance.StartDialogue(badLines);
    }
}

