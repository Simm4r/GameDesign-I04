using UnityEngine;
using UnityEngine.AI;

public class FleeingEntity : MonoBehaviour
{
    public enum EntityState { Idle, Fleeing, Safe }

    [SerializeField] private EntityState _currentState = EntityState.Idle;

    [Header("Settings")]
    [SerializeField] private Transform _den;
    [SerializeField] private float _detectionRange = 3f;
    [SerializeField] private float _pathRecalculateInterval = 1.2f;

    private NavMeshAgent _agent;
    private float _lastPathUpdateTime;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case EntityState.Idle:
                CheckPlayerDetection();
                break;
            case EntityState.Fleeing:
                FleeingUpdate();
                break;
            case EntityState.Safe:
                break;
        }
    }

    private void CheckPlayerDetection()
    {
        if (Player.Instance == null) return;

        float dist = Vector3.Distance(transform.position, Player.Instance.transform.position);
        if (dist <= _detectionRange && HasLineOfSightTo(Player.Instance.transform))
        {
            StartFleeing();
        }
    }

    private void StartFleeing()
    {
        _currentState = EntityState.Fleeing;
        _lastPathUpdateTime = -_pathRecalculateInterval; // forza aggiornamento immediato
        _agent.isStopped = false;
        _agent.SetDestination(_den.position);
    }

    private void FleeingUpdate()
    {
        if (Vector3.Distance(transform.position, _den.position) < _agent.stoppingDistance + 0.5f)
        {
            ReachDen();
            return;
        }

        float timeSinceUpdate = Time.time - _lastPathUpdateTime;
        if (timeSinceUpdate >= _pathRecalculateInterval)
        {
            _lastPathUpdateTime = Time.time;

            if (!HasLineOfSightTo(Player.Instance.transform))
            {
                _agent.isStopped = true;
            }
            else
            {
                if (_agent.isStopped)
                {
                    _agent.isStopped = false;
                    _agent.SetDestination(_den.position);
                }
            }
        }
    }

    private void ReachDen()
    {
        _currentState = EntityState.Safe;
        _agent.isStopped = true;
        transform.Find("Rat").gameObject.SetActive(false);
    }

    private bool HasLineOfSightTo(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = target.position - origin;
        float dist = Vector3.Distance(origin, target.position);

        if (dist > _detectionRange)
            return false;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist))
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }

        return false;
    }

    public void ResetAgent()
    {
        _agent.ResetPath();
        _currentState = EntityState.Idle;
        _agent.isStopped = false;
    }
}