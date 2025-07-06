using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CarveHandler : MonoBehaviour
{
    [SerializeField] private float _triggerDistance = 1f;
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
            
            foreach (NavMeshAgent agent in _agents)
            {
                if (Vector3.Distance(obstacle.transform.position, agent.transform.position) > _triggerDistance)
                    continue;
                find = true;
                obstacle.carving = true;
                agent.GetComponent<GuardPatrol>().ForcePathRecalculation();
                break;
            }
            if (!find)
                obstacle.carving = false;
        }
    }
}
