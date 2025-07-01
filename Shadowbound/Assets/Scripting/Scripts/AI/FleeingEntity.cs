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
    [SerializeField] private float _avoidStrength = 1.5f;

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
        _agent.isStopped = false;
        _lastPathUpdateTime = -_pathRecalculateInterval; // forza aggiornamento immediato
    }

    private void FleeingUpdate()
    {
        if (Vector3.Distance(transform.position, _den.position) < 1.5f)
        {
            ReachDen();
            return;
        }

        float timeSinceUpdate = Time.time - _lastPathUpdateTime;
        if (timeSinceUpdate >= _pathRecalculateInterval)
        {
            Vector3 fleeDir = (transform.position - Player.Instance.transform.position).normalized;
            Vector3 toDenDir = (_den.position - transform.position).normalized;

            // Direzione combinata: si allontana ma "scivola" verso la tana
            Vector3 moveDir = (fleeDir * _avoidStrength + toDenDir).normalized;

            Vector3 candidatePos = transform.position + moveDir * 5f;

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            _lastPathUpdateTime = Time.time;
        }
    }

    private void ReachDen()
    {
        _currentState = EntityState.Safe;
        _agent.isStopped = true;
        transform.Find("Capsule").gameObject.SetActive(false);
        transform.Find("Rat").gameObject.SetActive(false);
    }

    private bool HasLineOfSightTo(Transform target)
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = target.position - origin;
        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, _detectionRange))
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
