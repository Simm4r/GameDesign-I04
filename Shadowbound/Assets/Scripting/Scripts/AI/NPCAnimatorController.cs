using KinematicCharacterController;
using UnityEngine;
using UnityEngine.AI;

public class NPCAnimatorController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 2f;
    [SerializeField] private float _maxMotorSpeed = 1.9f;
    [SerializeField] private float _animationTransitionTime = 0.2f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private float _currentState = 0.0f;
    private float _currentVert = 0.0f;

    private void Awake()
    {
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        SetAnimationValues();
    }

    private void SetAnimationValues()
    {
        float currentSpeed;
        float referenceSpeed;

        bool isPossessed = PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity == gameObject;

        if (isPossessed)
        {
            var motor = GetComponent<KinematicCharacterMotor>();
            currentSpeed = motor.Velocity.magnitude;

            bool isEnhanced = gameObject.CompareTag("Possessable_Guard") && GetComponent<PossessedController>().Enhanced;
            referenceSpeed = isEnhanced ? 3f : 1.5f;

            _animator.speed = isEnhanced ? 2f : 1.30f;
        }
        else
        {
            currentSpeed = new Vector3(_agent.velocity.x, 0, _agent.velocity.z).magnitude;
            bool isEnhanced = gameObject.CompareTag("Possessable_Guard") && GetComponent<GuardStats>().Status == GuardStats.GuardStatus.Fastened;
            referenceSpeed = isEnhanced ? 2f : 1f;

            _animator.speed = isEnhanced ? 2f : 1f;
        }

        float normalizedSpeed = currentSpeed / referenceSpeed;

        if (normalizedSpeed > 0.97f) normalizedSpeed = 1f;
        normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0f, 2f);

        float targetState = currentSpeed > referenceSpeed * 1.2f ? 1f : 0f;
        _currentState = Mathf.Lerp(_currentState, targetState, Time.deltaTime / _animationTransitionTime);

        _currentVert = Mathf.Lerp(_currentVert, Mathf.Clamp01(normalizedSpeed), Time.deltaTime / _animationTransitionTime);

        _animator.SetFloat("State", _currentState);
        _animator.SetFloat("Vert", _currentVert);
    }
}