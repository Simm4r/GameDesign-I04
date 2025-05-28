using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GuardPatrol : MonoBehaviour
{
    public enum GuardState { Patrolling, Waiting, Alerted, Chasing, Investigating, Returning }
    [SerializeField] private GuardState _currentState = GuardState.Patrolling;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private int[] _waitPoints;

    [Header("Alert Settings")]
    [SerializeField] private Transform _target;
    [SerializeField] private float _alertRange = 8f;
    [SerializeField] private float _alertAngle = 120f;

    [Header("Chase Settings")]
    [SerializeField] private float _chaseSpeed = 2.1f;
    [SerializeField] private float _chaseDuration = 50f;
    [SerializeField] private float _stopDistance = 2f;

    [Header("Investigation Settings")]
    [SerializeField] private float _investigateDistance = 5f;

    private NavMeshAgent _agent;
    private float _walkingSpeed;
    private GameObject _exclamationMark;
    private GameObject _questionMark;
    private GuardPatrol[] _allGuards;

    private int _currentIndex = 0;
    private bool _goingForward = true;

    private float _stateTimer = 0f;
    private Vector3 _lastKnownPosition;
    private bool _isTargetVisible;

    private Vector3 _investigationPoint;
    private int _investigationPhase = 0;
    private float _lookAroundTimer = 0f;
    private float _nextLookDuration = 0f;
    private Quaternion _lookAroundRotation;
    private bool _isLookingAround = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _exclamationMark = transform.Find("ExclamationMark").gameObject;
        _questionMark = transform.Find("QuestionMark").gameObject;
        _walkingSpeed = _agent.speed;
        _allGuards = Object.FindObjectsByType<GuardPatrol>(FindObjectsSortMode.None);

        if (_waypoints != null && _waypoints.Length > 0)
        {
            _agent.SetDestination(_waypoints[_currentIndex].position);
        }
        else
        {
            _currentState = GuardState.Waiting;
            _agent.isStopped = true;
        }

        // HideMark();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case GuardState.Patrolling:       PatrolUpdate(); break;
            case GuardState.Waiting:          WaitingUpdate(); break;
            case GuardState.Alerted:          AlertedUpdate(); break;
            case GuardState.Chasing:          ChasingUpdate(); break;
            case GuardState.Investigating:    InvestigatingUpdate(); break;
            case GuardState.Returning:        ReturningUpdate(); break;
        }

        HandleVision();
    }

    // === PATROLLING ===
    private void PatrolUpdate()
    {
        if (_agent.pathPending) return;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (System.Array.Exists(_waitPoints, index => index == _currentIndex))
            {
                _currentState = GuardState.Waiting;
                _stateTimer = 0f;
                _agent.isStopped = true;
            }
            else
            {
                AdvanceToNextWaypoint();
            }
        }
    }

    private void WaitingUpdate()
    {
        if (_waypoints == null || _waypoints.Length == 0)
            return;

        _stateTimer += Time.deltaTime;
        if (_stateTimer >= _waitTime)
        {
            _agent.isStopped = false;
            AdvanceToNextWaypoint();
            _currentState = GuardState.Patrolling;
        }
    }

    private void AdvanceToNextWaypoint()
    {
        if (_waypoints == null || _waypoints.Length == 0)
            return;

        if (_goingForward)
        {
            if (++_currentIndex >= _waypoints.Length)
            {
                _currentIndex = _waypoints.Length - 2;
                _goingForward = false;
            }
        }
        else
        {
            if (--_currentIndex < 0)
            {
                _currentIndex = 1;
                _goingForward = true;
            }
        }

        _agent.ResetPath();
        _agent.SetDestination(_waypoints[_currentIndex].position);
    }

    // === VISION SYSTEM ===
    private void HandleVision()
    {
        if (_target == null) return;

        Vector3 eyePos = transform.position + Vector3.up * 1.25f + transform.forward * 0.2f;
        Vector3 dirToTarget = (_target.position - eyePos).normalized;
        float distToTarget = Vector3.Distance(eyePos, _target.position);

        // Calcolo angolo solo sul piano orizzontale
        Vector3 flatForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 flatDirToTarget = new Vector3(dirToTarget.x, 0, dirToTarget.z).normalized;
        float angleToTarget = Vector3.Angle(flatForward, flatDirToTarget);

        _isTargetVisible = false;

        // Se il target è vicino, allarga leggermente il campo visivo
        float dynamicAngle = _alertAngle / 2f;
        if (distToTarget < 1.5f)
            dynamicAngle *= 1.5f;

        if (distToTarget <= _alertRange && angleToTarget <= dynamicAngle)
        {
            if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hit, _alertRange))
            {
                // Debug.Log($"{_agent.name} - Hit: {hit.transform.root.name}");
                if (hit.transform == _target || hit.transform.IsChildOf(_target))
                {
                    _isTargetVisible = true;
                    _lastKnownPosition = _target.position;

                    if (_currentState == GuardState.Investigating)
                    {
                        StartChase();
                    }

                    if (_currentState != GuardState.Chasing && _currentState != GuardState.Alerted)
                    {
                        StartAlert();
                    }

                    FaceTarget();
                }
            }
        }
    }

    // === ALERTED ===
    private void StartAlert()
    {
        _currentState = GuardState.Alerted;
        _stateTimer = 0f;
        _agent.isStopped = true;
        ShowMark(_questionMark);
    }

    private void AlertedUpdate()
    {
        if (!_isTargetVisible)
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0)
                StartReturning();
        }
        else
        {
            _stateTimer += Time.deltaTime;

            if (_stateTimer > 1.5f && _isTargetVisible)
            {
                StartChase();
            }
        }
    }

    // === CHASING ===
    private void StartChase()
    {
        _currentState = GuardState.Chasing;
        _stateTimer = 0f;
        _agent.stoppingDistance = _stopDistance;
        _agent.isStopped = false;
        _agent.speed = _chaseSpeed;
        ShowMark(_exclamationMark);
    }

    private void ChasingUpdate()
    {
        _stateTimer += Time.deltaTime;

        if (IsClosestGuardToTarget())
        {
            if (_isTargetVisible)
            {
                float dist = Vector3.Distance(transform.position, _target.position);
                if (dist > _stopDistance)
                    _agent.SetDestination(_target.position);
                else
                    _agent.ResetPath();

                _lastKnownPosition = _target.position;
            }
            else if (_stateTimer < _chaseDuration)
            {
                _agent.SetDestination(_lastKnownPosition);

                if (Vector3.Distance(transform.position, _lastKnownPosition) <= _agent.stoppingDistance)
                {
                    StartInvestigation();
                }
            }
            else
            {
                StartInvestigation();
            }
        }
        else
        {
            Vector3 toLeader = _target.position - transform.position;
            Vector3 followPoint = _target.position - toLeader.normalized * 1f;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(followPoint, out hit, 1f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }

    private bool IsClosestGuardToTarget()
    {
        float myDist = Vector3.Distance(transform.position, _target.position);

        foreach (var guard in _allGuards)
        {
            if (guard == this) continue;
            if (Vector3.Distance(guard.transform.position, _target.position) < myDist)
                return false;
        }
        return true;
    }

    // === INVESTIGATING ===
    private void StartInvestigation()
    {
        _currentState = GuardState.Investigating;
        _agent.isStopped = true;
        _stateTimer = 0f;
        _investigationPhase = 0;
        _agent.speed = _walkingSpeed;
        ShowMark(_questionMark);
    }

    private void InvestigatingUpdate()
    {
        _stateTimer += Time.deltaTime;

    switch (_investigationPhase)
        {
            case 0: // Guarda attorno
                if (_stateTimer >= 3f)
                {
                    _stateTimer = 0f;
                    _investigationPoint = GetRandomPointNear(_lastKnownPosition, _investigateDistance);
                    _agent.SetDestination(_investigationPoint);
                    _agent.isStopped = false;
                    _investigationPhase = 1;
                }
                else
                {
                    HandleLookAround();
                }
                break;

            case 1: // Cammina verso il punto
                if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
                {
                    _agent.isStopped = true;
                    _stateTimer = 0f;
                    _investigationPhase = 2;
                }
                break;

            case 2: // Guarda attorno
                if (_stateTimer >= 3f)
                {
                    _investigationPhase = 3;
                    StartReturning();
                }
                else
                {
                    HandleLookAround();
                }
                break;
        }
    }

    private void HandleLookAround()
    {
        _lookAroundTimer += Time.deltaTime;

        if (!_isLookingAround || _lookAroundTimer >= _nextLookDuration)
        {
            _lookAroundTimer = 0f;
            _nextLookDuration = Random.Range(1f, 2f); // Quanto tempo guarderà in quella direzione

            float[] angles = new float[] { -90f, 90f, 180f, -45f, 45f };
            float randomYaw = angles[Random.Range(0, angles.Length)];

            _lookAroundRotation = Quaternion.Euler(0f, transform.eulerAngles.y + randomYaw, 0f);
            _isLookingAround = true;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _lookAroundRotation, Time.deltaTime * 2f);

        if (Quaternion.Angle(transform.rotation, _lookAroundRotation) < 1f)
        {
            _isLookingAround = false;
        }
    }

    // === RETURNING ===
    private void StartReturning()
    {
        _stateTimer = 0f;
        _agent.isStopped = false;
        _currentState = GuardState.Returning;
        _agent.ResetPath();
        _agent.SetDestination(_waypoints[_currentIndex].position);
        HideMark();
    }
    private void ReturningUpdate()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            _currentState = GuardState.Patrolling;
        }
    }

    // === UTILS === 
    private void ShowMark(GameObject markToShow)
    {
        _exclamationMark.SetActive(markToShow == _exclamationMark);
        _questionMark.SetActive(markToShow == _questionMark);
    }

    private void HideMark()
    {
        _exclamationMark.SetActive(false);
        _questionMark.SetActive(false);
    }

    private void FaceTarget()
    {
        Vector3 point = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        RotateTowards(point);
    }

    private void RotateTowards(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2f);
    }

    private Vector3 GetRandomPointNear(Vector3 origin, float radius)
    {
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 point = origin + new Vector3(randomCircle.x, 0, randomCircle.y);

        // Verifica che il punto sia sul NavMesh
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return origin;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 eyePos = transform.position + Vector3.up * 1.25f  + transform.forward * 0.2f;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(eyePos, 0.1f);

        // Colore del cono
        Gizmos.color = Color.yellow;

        // Numero di linee per visualizzare il cono
        int segments = 30;
        float halfAngle = _alertAngle * 0.5f;

        // Rotazione iniziale
        Quaternion leftRayRotation = Quaternion.AngleAxis(-halfAngle, Vector3.up);
        Quaternion stepRotation = Quaternion.AngleAxis(_alertAngle / segments, Vector3.up);

        Vector3 direction = transform.forward;
        Vector3 currentDir = leftRayRotation * direction;

        Vector3 prevPoint = origin + currentDir * _alertRange;

        for (int i = 0; i <= segments; i++)
        {
            Vector3 nextDir = stepRotation * currentDir;
            Vector3 nextPoint = origin + nextDir * _alertRange;

            Gizmos.DrawLine(origin, prevPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);

            currentDir = nextDir;
            prevPoint = nextPoint;
        }

        // Linea verso il target, se disponibile
        if (_target != null)
        {
            Gizmos.color = _isTargetVisible ? Color.red : Color.green;
            Gizmos.DrawLine(eyePos, _target.position);
        }
    }

}