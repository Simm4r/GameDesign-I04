using System.Collections;
using UnityEngine;

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

        var missile = GetComponentInChildren<MagicMissleController>();
        missile.TrialPos = _target.position + _target.forward * 0.5f;
        missile.TryLaunch = true;
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

    protected override void CheckOddity()
    {
        _currentState = GuardState.Chasing;
        _stateTimer = 0f;
        _agent.isStopped = false;
        _agent.speed = _chaseSpeed;
        _agent.avoidancePriority = 10;
        ShowMark(_exclamationMark);

        if (Vector3.Distance(transform.position, _target.position) < 1f)
        {
            InteractWithObstacle(_target.GetComponent<Interactable>());
        }
    }

    public override void ResetAgent()
    {
        _agent.ResetPath();
        _currentState = GuardState.Patrolling;
        HideMark();
        _agent.speed = _originalWalkSpeed;
        _agent.avoidancePriority = _originalPriority;
        _agent.isStopped = false;
    }
}
