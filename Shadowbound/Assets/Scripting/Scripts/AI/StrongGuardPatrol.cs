using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class StrongGuardPatrol : GuardPatrol
{
    [Header("Strong Guard Settings")]
    [SerializeField] private SphereLauncher _sphere;

    private Animator _anim;

    protected override void HandleVision()
    {
        base.HandleVision();

        if (_currentState == GuardState.Chasing && _target == Player.Instance.transform && !_sphere.OnField)
        {
            StartCoroutine(MissileRoutine());
        }
    }

    private IEnumerator MissileRoutine()
    {
        float delay = Random.Range(3f, 5f);
        yield return new WaitForSeconds(delay);

        if (_currentState == GuardState.Chasing)
        {
            var missile = GetComponentInChildren<MagicMissleController>();
            missile.TrialPos = _target.position + _target.forward * 0.5f;
            missile.TryLaunch = true;
        }
    }

    protected override Interactable GetObstacleToInteract()
    {
        // Prima prova con la logica base
        Interactable baseObstacle = base.GetObstacleToInteract();
        if (baseObstacle != null) return baseObstacle;

        // Poi cerca barili da rompere
        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

        foreach (var hit in hits)
        {
            if (hit.transform.root.GetComponent<DestroyBarrel>() != null)
            {
                return hit.transform.root.GetComponent<Interactable>();
            }
        }

        return null;
    }

    protected override void InteractWithObstacle(Interactable obstacle)
    {
        if (obstacle is DestroyBarrel)
        {
            RotateTowards(obstacle.transform.position);
            StartCoroutine(PlayAttackAndInteract(obstacle));
        }
        else
        {
            base.InteractWithObstacle(obstacle);
        }
    }

    private IEnumerator PlayAttackAndInteract(Interactable obstacle)
    {
        _anim = GetComponent<Animator>();
        _anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.3f);

        base.InteractWithObstacle(obstacle);
    }

    protected override void StartCheckingOddity()
    {
        _currentState = GuardState.Checking;
        _stateTimer = 0f;
        _agent.isStopped = false;
        _agent.speed = _chaseSpeed;
        _agent.avoidancePriority = 10;
        ShowMark(_exclamationMark);
    }

    protected override void CheckingUpdate()
    {
        _stateTimer += Time.deltaTime;

        if (_isTargetVisible && _target != Player.Instance.transform)
        {
            float dist = Vector3.Distance(transform.position, _target.position);
            if (dist > 0.5f)
            {
                if (!NavMesh.SamplePosition(_target.position, out _, 1f, NavMesh.AllAreas))
                {
                    StartInvestigation();
                }

                _agent.SetDestination(_target.position);
            }
            else if (_target.GetComponent<MultiTag>() == null) // Lo specchio ha un multi-tag
                InteractWithObstacle(_target.GetComponent<Interactable>());

            _lastKnownPosition = _target.position;
            _targetDirection = _targetMotor.Velocity.magnitude > 0.1f ? _targetMotor.Velocity.normalized : null;
        }
        else if (_stateTimer < _checkDuration)
        {
            if (!NavMesh.SamplePosition(_lastKnownPosition, out _, 1f, NavMesh.AllAreas) || Vector3.Distance(transform.position, _lastKnownPosition) <= _agent.stoppingDistance + 1f)
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

    /*public override void ResetAgent()
    {
        _agent.ResetPath();
        _currentState = GuardState.Patrolling;
        HideMark();
        _agent.speed = _originalWalkSpeed;
        _agent.avoidancePriority = _originalPriority;
        _agent.isStopped = false;
    }*/
}
