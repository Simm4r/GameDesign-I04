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
        Debug.Log(PossessionHandler.Instance.PossessedEntity);
        _maxSpeed =(PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity == gameObject)  ? _maxMotorSpeed : _maxSpeed;
            
        float currentSpeed = (PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity == gameObject) ? GetComponent<KinematicCharacterMotor>().Velocity.magnitude : new Vector3(_agent.velocity.x, 0, _agent.velocity.z).magnitude;
        float normalizedSpeed = Mathf.Clamp01(currentSpeed / _maxSpeed); // Valore tra 0 e 1

        // State = camminata o corsa, interpolata (usa threshold ~0.8f)
        float targetState = normalizedSpeed > 0.8f ? 1f : 0f;
        _currentState = Mathf.Lerp(_currentState, targetState, Time.deltaTime / _animationTransitionTime);

        // Vert = blending tra idle e movimento
        _currentVert = Mathf.Lerp(_currentVert, normalizedSpeed, Time.deltaTime / _animationTransitionTime);

        _animator.SetFloat("State", _currentState);
        _animator.SetFloat("Vert", _currentVert);
    }
}