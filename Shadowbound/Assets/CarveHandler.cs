using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarveHandler : MonoBehaviour
{
    private List<NavMeshAgent> _agents = new();
    private List<NavMeshObstacle> _obstacles = new();

    void Awake()
    {
        _agents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList().Where(agent => agent.transform.root.CompareTag("Possessable_Guard")).ToList();
        _obstacles = FindObjectsByType<NavMeshObstacle>(FindObjectsSortMode.None).ToList();
        _obstacles.ForEach(obstacle => obstacle.carving = false);
    }

    void Update()
    {
        foreach (NavMeshObstacle obstacle in _obstacles)
        {
            bool find = false;
            if (obstacle == null) continue;

            float triggerDistance = 1.5f; // default
            bool isDoor = obstacle.GetComponent<DoorOpener>() != null;
            
            foreach (NavMeshAgent agent in _agents)
            {
                if (isDoor)
                {
                    Vector3 toAgent = (agent.transform.position - obstacle.transform.position).normalized;
                    Vector3 forward = obstacle.transform.forward;
                    float dot = Vector3.Dot(forward, toAgent);

                    if (dot > 0.5f) // la porta si apre verso l'agente
                        triggerDistance = 2.5f;
                    else
                        triggerDistance = 1f;
                }

                if (Vector3.Distance(obstacle.transform.position, agent.transform.position) > triggerDistance)
                    continue;
                find = true;

                if (!obstacle.carving)
                {
                    obstacle.carving = true;
                    if (agent.enabled &&
                        (obstacle.GetComponent<BarrelStats>() != null ||
                        (obstacle.GetComponent<DoorOpener>() != null && !obstacle.GetComponent<DoorOpener>().IsOpen && !obstacle.GetComponent<DoorOpener>().IsAnimationStarted) ||
                        (obstacle.GetComponent<PortcullisHandler>() != null && !obstacle.GetComponent<PortcullisHandler>().IsUp && !obstacle.GetComponent<PortcullisHandler>().IsActive)))
                        agent.GetComponent<GuardPatrol>().ForcePathRecalculation();
                }

                break;
            }
            if (!find)
                obstacle.carving = false;
        }
    }
}
