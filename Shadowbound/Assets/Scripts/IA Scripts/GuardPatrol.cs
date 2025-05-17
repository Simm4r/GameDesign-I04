using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GuardPatrol : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;          // Punti da seguire
    [SerializeField] private float waitTime = 2f;            // Tempo di pausa in certi punti
    [SerializeField] private int[] waitPoints;               // Indici dei waypoint dove aspettare

    private NavMeshAgent agent;
    private Animator animator;
    private int currentIndex = 0;
    private bool goingForward = true;
    private bool isWaiting = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[currentIndex].position);
    }

    void Update()
    {
        if (agent.pathPending || isWaiting)
            return;

        float dist = Vector3.Distance(transform.position, agent.destination);
        animator.SetBool("isWalking", true);

        if (dist < 0.3f)
        {
            StartCoroutine(WaitAndGo());
        }
    }

    IEnumerator WaitAndGo()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);

        // Controlla se il punto corrente è un punto di attesa
        bool shouldWait = System.Array.Exists(waitPoints, index => index == currentIndex);
        if (shouldWait)
            yield return new WaitForSeconds(waitTime);

        // Cambia direzione se siamo agli estremi
        if (goingForward)
        {
            if (currentIndex < waypoints.Length - 1)
                currentIndex++;
            else
            {
                goingForward = false;
                currentIndex--;
            }
        }
        else
        {
            if (currentIndex > 0)
                currentIndex--;
            else
            {
                goingForward = true;
                currentIndex++;
            }
        }

        agent.SetDestination(waypoints[currentIndex].position);
        isWaiting = false;
    }
}