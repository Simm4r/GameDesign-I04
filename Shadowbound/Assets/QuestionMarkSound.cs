using UnityEngine;

public class ExclamationMarkSound : MonoBehaviour
{
    [SerializeField] private AudioSource _source;
    [SerializeField] private GuardPatrol _patrol;
    GuardPatrol.GuardState _state = GuardPatrol.GuardState.Patrolling;
    private bool _foundSound = false;

    void Update()
    {
        if (!_patrol.enabled)
            return;

        if (_state != _patrol.CurrentState)
        {
            _state = _patrol.CurrentState;
            if (_state == GuardPatrol.GuardState.Chasing && !_foundSound)
            {
                _foundSound = true;
                _source.Play();
            }
            if (_state == GuardPatrol.GuardState.Returning || _state == GuardPatrol.GuardState.Patrolling)
                _foundSound = false;
        }
    }
}
