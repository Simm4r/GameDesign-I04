using UnityEngine;

public class StrongGuardPatrol : GuardPatrol
{
    [Header("Strong Guard Settings")]
    [SerializeField] private SphereLauncher _sphere;

    protected override void HandleVision()
    {
        base.HandleVision();

        if (_currentState == GuardState.Chasing && _target == Player.Instance.transform)
        {
            if (!_sphere.OnField)
            {
                GetComponentInChildren<MagicMissleController>().TrialPos = _target.position + _target.forward * 0.5f;
                GetComponentInChildren<MagicMissleController>().TryLaunch = true;
            }
        }
    }
}
