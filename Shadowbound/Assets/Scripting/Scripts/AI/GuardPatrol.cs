using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using KinematicCharacterController;
using System.Linq;

public class GuardPatrol : MonoBehaviour
{
    public enum GuardState { Patrolling, Waiting, Alerted, Chasing, Investigating, Returning, Checking }
    [SerializeField] protected GuardState _currentState = GuardState.Patrolling;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private int[] _waitPoints;

    [Header("Alert Settings")]
    [SerializeField] private float _alertRange = 8f;
    [SerializeField] private float _alertAngle = 120f;

    [Header("Chase Settings")]
    [SerializeField] protected float _chaseSpeed = 2.1f;
    [SerializeField] private float _chaseDuration = 20f;
    [SerializeField] private float _stopDistance = 1f;

    [Header("Checking Oddity Settings")]
    [SerializeField] protected float _checkDuration = 10f;

    [Header("Investigation Settings")]
    [SerializeField] private float _investigateDistance = 5f;

    [Header("Status Scared")]
    [SerializeField] private Transform _secretRoom;
    [SerializeField] private Book _book;
    [SerializeField] private HiddenWallLift _secretRoomWall;

    protected Transform _target;
    protected NavMeshAgent _agent;
    private GuardStats _stats;
    private GuardStats.GuardStatus _currentStatus = GuardStats.GuardStatus.None;
    public GuardState CurrentState {
        get => _currentState;
    }
    protected KinematicCharacterMotor _targetMotor;
    protected int _originalPriority;
    private float _rotationSpeed = 2f;
    private float _originalRotSpeed;
    protected float _walkingSpeed;
    protected float _originalWalkSpeed;
    protected float _originalChaseSpeed;
    private bool _isHandlingObstacle = false;
    protected GameObject _exclamationMark;
    protected GameObject _questionMark;
    private QuestionMarkFiller _markFiller;
    private GuardPatrol[] _allGuards;

    private int _currentIndex = 0;
    private bool _goingForward = true;

    protected float _stateTimer = 0f;
    protected Vector3 _lastKnownPosition;
    protected bool _isTargetVisible;

    private Vector3 _investigationPoint;
    private int _investigationPhase = 0;
    private float _lookAroundTimer = 0f;
    private bool _firstLook = true;
    protected Vector3? _targetDirection = null;
    private float _nextLookDuration = 0f;
    private Quaternion _lookAroundRotation;
    private bool _isLookingAround = false;

    private bool _firstEnable = true;
    private bool _enableAfterDialogue = false;

    public bool EnableAfterDialogue
    {
        get => _enableAfterDialogue;
        set => _enableAfterDialogue = value;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _exclamationMark = transform.Find("ExclamationMark").gameObject;
        _questionMark = transform.Find("QuestionMark").gameObject;
        _markFiller = GetComponentInChildren<QuestionMarkFiller>(true);
        _walkingSpeed = _agent.speed;
        _originalWalkSpeed = _walkingSpeed;
        _originalChaseSpeed = _chaseSpeed;
        _originalRotSpeed = _rotationSpeed;
        _originalPriority = _agent.avoidancePriority;
        _allGuards = FindObjectsByType<GuardPatrol>(FindObjectsSortMode.None);

        if (_waypoints == null || _waypoints.Length == 0)
        {
            _currentState = GuardState.Waiting;
            _agent.isStopped = true;
        }
        else if (_waypoints.Length == 1)
        {
            _agent.SetDestination(_waypoints[0].position);
        }
        else
        {
            _agent.SetDestination(_waypoints[_currentIndex].position);
        }

        // HideMark();
    }

    private void Start()
    {
        _target = Player.Instance.transform;
        _targetMotor = _target.GetComponent<KinematicCharacterMotor>();
    }

    private void OnEnable()
    {
        _stats = GetComponent<GuardStats>();
        _stats.OnStatusChanged += HandleStatusChange;

        HandleStatusChange(_stats.Status);

        if (_currentStatus != GuardStats.GuardStatus.Scared && _currentStatus != GuardStats.GuardStatus.Sleepy)
        {
            if (_firstEnable)
            {
                _firstEnable = false;
                return;
            }
            if (_enableAfterDialogue)
            {
                _enableAfterDialogue = false;
                return;
            }
            StartInvestigation(2);
        }
    }

    private void OnDisable()
    {
        _stats.OnStatusChanged -= HandleStatusChange;          
    }

    private void HandleStatusChange(GuardStats.GuardStatus status)
    {
        _currentStatus = status;

        switch (status)
        {
            case GuardStats.GuardStatus.Sleepy:
                _agent.isStopped = true;
                break;

            case GuardStats.GuardStatus.Fastened:
                _walkingSpeed *= 2f;
                _chaseSpeed *= 2f;
                _rotationSpeed *= 2f;
                _agent.avoidancePriority = 5;
                break;

            case GuardStats.GuardStatus.Scared:
                StartCoroutine(HandleScaredSequence());
                break;
            case GuardStats.GuardStatus.None:
                _walkingSpeed = _originalWalkSpeed;
                _chaseSpeed = _originalChaseSpeed;
                _rotationSpeed = _originalRotSpeed;
                _agent.avoidancePriority = _originalPriority;
                _agent.isStopped = false;
                break;
        }
    }

    private void Update()
    {
        if (_currentStatus == GuardStats.GuardStatus.Sleepy || _currentStatus == GuardStats.GuardStatus.Scared) return;

        if (PlayerInput.Instance.InPossession)
        {
            if (PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Object")
            {
                if (_target != PossessionHandler.Instance.PossessedEntity.transform)
                {
                    _target = PossessionHandler.Instance.PossessedEntity.transform;
                    _targetMotor = _target.GetComponent<KinematicCharacterMotor>();
                }
            }
            else
            {
                _target = null;
            }
        }

        if (!PlayerInput.Instance.InPossession && _target != Player.Instance.transform)
        {
            _target = Player.Instance.transform;
            _targetMotor = _target.GetComponent<KinematicCharacterMotor>();
        }

        switch (_currentState)
        {
            case GuardState.Patrolling: PatrolUpdate(); break;
            case GuardState.Waiting: WaitingUpdate(); break;
            case GuardState.Alerted: AlertedUpdate(); break;
            case GuardState.Chasing: ChasingUpdate(); break;
            case GuardState.Investigating: InvestigatingUpdate(); break;
            case GuardState.Returning: ReturningUpdate(); break;
            case GuardState.Checking: CheckingUpdate(); break;
        }

        HandleVision();
    }

    // === PATROLLING ===
    private void PatrolUpdate()
    {
        if (_agent.pathPending) return;

        if (_agent.speed == _chaseSpeed)
        {
            _agent.speed = _walkingSpeed;
            _agent.avoidancePriority = _originalPriority;
        }

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
        if (_waypoints == null || _waypoints.Length <= 1)
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

        if (_waypoints.Length == 1)
        {
            _agent.SetDestination(_waypoints[0].position);
            return;
        }

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
    protected virtual void HandleVision()
    {
        if (_target == null) return;
        
        if (_currentStatus == GuardStats.GuardStatus.Scared || _currentStatus == GuardStats.GuardStatus.Sleepy) return;

        Vector3 eyePos = transform.position + Vector3.up * 1.25f + transform.forward * 0.2f;
        Vector3 dirToTarget = (_target.position - eyePos).normalized;
        float distToTarget = Vector3.Distance(eyePos, _target.position);

        // Calcolo angolo solo sul piano orizzontale
        Vector3 flatForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 flatDirToTarget = new Vector3(dirToTarget.x, 0, dirToTarget.z).normalized;
        float angleToTarget = Vector3.Angle(flatForward, flatDirToTarget);

        float objectVelocity;

        _isTargetVisible = false;

        // Se il target è vicino, allarga leggermente il campo visivo
        float dynamicAngle = _alertAngle / 2f;
        if (distToTarget < 1.5f)
            dynamicAngle *= 1.5f;

        if (distToTarget <= _alertRange && angleToTarget <= dynamicAngle)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
            Collider smokeCollider = colliders.FirstOrDefault(c => c.CompareTag("Smoke"));

            if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hit, _alertRange) && smokeCollider == null)
            {
                // Debug.Log($"{_agent.name} - Hit: {hit.transform.root.name}");
                if (hit.transform == _target || hit.transform.IsChildOf(_target))
                {
                    _isTargetVisible = true;
                    _lastKnownPosition = _target.position;

                    if (_target != Player.Instance.transform)
                    {
                        if (_currentState != GuardState.Checking)
                        {
                            objectVelocity = _targetMotor.Velocity.magnitude;
                            if (objectVelocity == 0) return;
                            StartCheckingOddity();
                        }
                    }
                    else
                    {
                        if (_markFiller.IsVisible() && Mathf.Approximately(_markFiller.GetCurrentFill(), 1f))
                        {
                            StartChase();
                        }

                        if (_currentState != GuardState.Chasing && _currentState != GuardState.Alerted)
                        {
                            StartAlert();
                        }
                    }

                    FaceTarget();
                }
            }
            else if (smokeCollider != null)
            {
                StartInvestigation();
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
        if (_target == null || _target != Player.Instance.transform)
        {
            StartReturning();
            return;
        }

        if (!_isTargetVisible)
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0)
                _markFiller.ResetFill();
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

        _markFiller.SetFill(_stateTimer);
    }

    // === CHASING ===
    private void StartChase()
    {
        _currentState = GuardState.Chasing;
        _stateTimer = 0f;
        _agent.isStopped = false;
        _agent.speed = _chaseSpeed;
        _agent.avoidancePriority = 10;
        ShowMark(_exclamationMark);
    }

    private void ChasingUpdate()
    {
        if (_target == null || _target != Player.Instance.transform)
        {
            StartInvestigation();
            return;
        }

        _stateTimer += Time.deltaTime;

        if (IsClosestGuardToTarget())
        {
            if (_isTargetVisible && _target == Player.Instance.transform)
            {
                float dist = Vector3.Distance(transform.position, _target.position);
                if (dist > _stopDistance)
                {
                    if (!NavMesh.SamplePosition(_target.position, out _, 1f, NavMesh.AllAreas))
                    {
                        StartInvestigation();
                    }

                    _agent.SetDestination(_target.position);
                }
                else
                    _agent.ResetPath();

                _lastKnownPosition = _target.position;
                _targetDirection = _targetMotor.Velocity.magnitude > 0.1f ? _targetMotor.Velocity.normalized : null;
            }
            else if (_stateTimer < _chaseDuration)
            {
                if (!NavMesh.SamplePosition(_lastKnownPosition, out _, 1f, NavMesh.AllAreas) || Vector3.Distance(transform.position, _lastKnownPosition) <= _agent.stoppingDistance)
                {
                    StartInvestigation();
                }

                _agent.SetDestination(_lastKnownPosition);
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
            
            if (!NavMesh.SamplePosition(followPoint, out _, 1f, NavMesh.AllAreas))
            {
                StartInvestigation();
            }

            _agent.SetDestination(followPoint);
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
    protected void StartInvestigation(int phase = 0)
    {
        _currentState = GuardState.Investigating;
        _agent.isStopped = true;
        _stateTimer = 0f;
        _investigationPhase = phase;
        _agent.speed = _walkingSpeed;
        _agent.avoidancePriority = _originalPriority;
        _markFiller.SetMaxFill();
        ShowMark(_questionMark);
        _firstLook = true;
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
                    _investigationPoint = GetRandomPointNear(_lastKnownPosition, _investigateDistance, _targetMotor.Velocity.magnitude > 0.1f ? _targetMotor.Velocity.normalized : null);
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
                if (_stateTimer >= 4f)
                {
                    _investigationPhase = 3;
                    _markFiller.ResetFill();
                    StartReturning();
                }
                else
                {
                    if (_stateTimer >= 2f)
                    {
                        HandleLookAround();
                    }
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

            float targetYaw;

            if (_firstLook && _targetDirection.HasValue)
            {
                // Guardare nella direzione della velocità del target
                Vector3 dir = _targetDirection.Value;
                targetYaw = Quaternion.LookRotation(dir).eulerAngles.y;
                _firstLook = false;
            }
            else
            {
                // Scelta casuale
                float[] angles = new float[] { -90f, 90f, 180f, -45f, 45f };
                float randomYaw = angles[Random.Range(0, angles.Length)];
                targetYaw = transform.eulerAngles.y + randomYaw;
            }

            _lookAroundRotation = Quaternion.Euler(0f, targetYaw, 0f);
            _isLookingAround = true;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, _lookAroundRotation, Time.deltaTime * _rotationSpeed);

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

        if (_waypoints == null || _waypoints.Length == 0)
        {
            _currentState = GuardState.Waiting;
            _agent.ResetPath();
            HideMark();
            return;
        }

        _currentState = GuardState.Returning;
        _agent.ResetPath();
        _agent.SetDestination(_waypoints[_currentIndex].position);
        HideMark();
    }
    private void ReturningUpdate()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            if (_waypoints.Length <= 1)
            {
                _currentState = GuardState.Waiting;
                _agent.isStopped = true;
            }
            else
            {
                _currentState = GuardState.Patrolling;
            }
        }
    }

    // === CHECK ODDITY ===
    protected virtual void StartCheckingOddity()
    {
        _currentState = GuardState.Checking;
        _stateTimer = 0f;
        _agent.isStopped = false;
        _agent.speed = _walkingSpeed;
        _agent.avoidancePriority = _originalPriority;
        _markFiller.SetMaxFill();
        ShowMark(_questionMark);
    }

    protected virtual void CheckingUpdate()
    {
        if (_target == null)
        {
            StartInvestigation();
            return;
        }

        _stateTimer += Time.deltaTime;

        if (IsClosestGuardToTarget())
        {
            if (_isTargetVisible && _target != Player.Instance.transform)
            {
                float dist = Vector3.Distance(transform.position, _target.position);
                if (dist > _stopDistance)
                {
                    if (!NavMesh.SamplePosition(_target.position, out _, 1f, NavMesh.AllAreas))
                    {
                        StartInvestigation();
                    }

                    _agent.SetDestination(_target.position);
                }
                else
                    _agent.ResetPath();

                _lastKnownPosition = _target.position;
                _targetDirection = _targetMotor.Velocity.magnitude > 0.1f ? _targetMotor.Velocity.normalized : null;
            }
            else if (_stateTimer < _checkDuration)
            {
                if (!NavMesh.SamplePosition(_lastKnownPosition, out _, 1f, NavMesh.AllAreas) || Vector3.Distance(transform.position, _lastKnownPosition) <= _agent.stoppingDistance)
                {
                    StartInvestigation();
                }

                _agent.SetDestination(_lastKnownPosition);
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
            
            if (!NavMesh.SamplePosition(followPoint, out _, 1f, NavMesh.AllAreas))
            {
                StartInvestigation();
            }

            _agent.SetDestination(followPoint);
        }
    }

    // === UTILS === 
    protected void ShowMark(GameObject markToShow)
    {
        _exclamationMark.SetActive(markToShow == _exclamationMark);
        _questionMark.SetActive(markToShow == _questionMark);
    }

    protected void HideMark()
    {
        _exclamationMark.SetActive(false);
        _questionMark.SetActive(false);
    }

    private void FaceTarget()
    {
        Vector3 point = new Vector3(_target.position.x, transform.position.y, _target.position.z);
        RotateTowards(point);
    }

    protected void RotateTowards(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
    }

    private Vector3 GetRandomPointNear(Vector3 origin, float distance, Vector3? direction = null)
    {
        const int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 finalDir = direction ?? Random.insideUnitSphere;
            finalDir.y = 0f;

            Vector2 randomOffset = Random.insideUnitCircle * (distance * 0.3f);
            Vector3 offset = new Vector3(randomOffset.x, 0, randomOffset.y);

            Vector3 desiredPoint = origin + finalDir.normalized * distance + offset;

            // Controlla che sia sul NavMesh
            if (NavMesh.SamplePosition(desiredPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                // Verifica che il percorso sia completo
                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(origin, hit.position, NavMesh.AllAreas, path)
                    && path.status == NavMeshPathStatus.PathComplete)
                {
                    return hit.position;
                }
            }
        }

        // Fallback: restituisci l'origine se nessun punto valido trovato
        return origin;
    }

    private IEnumerator HandleScaredSequence()
    {
        _agent.isStopped = false;
        _agent.speed = _chaseSpeed;
        _agent.avoidancePriority = 12;

        // Se il muro è già alzato, salta direttamente al secondo punto
        if (_secretRoomWall.State == HiddenWallLift.WallState.Up)
        {
            if (_secretRoom != null)
            {
                _agent.SetDestination(_secretRoom.position);
                while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
                    yield return null;

                StartCoroutine(ScaredBackAndForth());
            }
            yield break;
        }

        // Fase 1: fugge verso il primo punto
        if (_book != null)
        {
            _agent.SetDestination(_book.transform.position);
            while (_agent.pathPending || _agent.remainingDistance > 1f)
                yield return null;
        }

        // Fase 2: chiama StartAnimation sul libro
        _book?.StartAnimation();

        // Fase 3: attende che il muro sia salito
        while (_secretRoomWall.State != HiddenWallLift.WallState.Up)
            yield return null;

        // Fase 4: fugge verso il secondo punto
        if (_secretRoom != null)
        {
            _agent.SetDestination(_secretRoom.position);
            while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
                yield return null;

            StartCoroutine(ScaredBackAndForth());
        }
    }

    private IEnumerator ScaredBackAndForth()
    {
        Vector3 start = _secretRoom.position;
        Vector3 dir = Vector3.forward; // asse z
        bool forward = true;

        while (_currentStatus == GuardStats.GuardStatus.Scared)
        {
            Vector3 target = start + dir * (forward ? 1f : -1f);

            _agent.SetDestination(target);

            while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance)
                yield return null;

            forward = !forward;
        }
    }

    public void ForcePathRecalculation()
    {
        if (!_isHandlingObstacle)
            StartCoroutine(HandleStuckAndInteract());
    }

    private IEnumerator HandleStuckAndInteract()
    {
        _isHandlingObstacle = true;

        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(_agent.destination, path);

        if (path.corners.Length >= 2)
        {
            Vector3 from = path.corners[0];
            Vector3 to = path.corners[1];
            Vector3 pathDir = (to - from).normalized;

            // Ottieni l’ostacolo potenzialmente interagibile
            Interactable obstacle = GetObstacleToInteract();

            if (obstacle != null)
            {
                Vector3 obstaclePos;
                if (obstacle.GetComponent<PullLeverHandler>() != null)
                {
                    obstaclePos = obstacle.GetComponent<PullLeverHandler>().Portcullis.transform.position;
                }
                else
                {
                    obstaclePos = obstacle.transform.position;
                }
                
                Vector3 obstacleDir = (obstaclePos - _agent.transform.position).normalized;
                float dot = Vector3.Dot(pathDir, obstacleDir);

                // Se la direzione dell'ostacolo è abbastanza allineata col percorso (es: almeno 0.7 su 1)
                if (dot > 0.7f)
                {
                    InteractWithObstacle(obstacle);

                    yield return new WaitForSeconds(1f);
                    _agent.SetDestination(_agent.destination);
                }
            }
        }

        _isHandlingObstacle = false;
    }

    protected virtual Interactable GetObstacleToInteract()
    {
        Transform targetTransform = null;
        float checkRadius = 3f;
        Collider[] hits = Physics.OverlapSphere(transform.position, checkRadius);

        foreach (var hit in hits)
        {
            if (hit.GetComponent<DoorOpener>() != null)
            {
                if (hit.GetComponent<DoorLocked>() != null && hit.GetComponent<DoorLocked>().IsLocked)
                    continue;

                targetTransform = hit.transform;
                break;
            }
            else if (hit.transform.parent != null && hit.transform.parent.GetComponent<PullLeverHandler>() != null)
            {
                targetTransform = hit.transform.parent;
                break;
            }
        }

        return targetTransform?.GetComponent<Interactable>();
    }

    protected virtual void InteractWithObstacle(Interactable obstacle)
    {
        obstacle.Interact();
    }

    public virtual void ResetAgent()
    {
        _agent.ResetPath();
        _currentIndex = 0;
        _currentState = GuardState.Patrolling;
        HideMark();
        _agent.speed = _originalWalkSpeed;
        _agent.avoidancePriority = _originalPriority;
        _agent.isStopped = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position;
        Vector3 eyePos = transform.position + Vector3.up * 1.25f + transform.forward * 0.2f;
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