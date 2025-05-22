using KinematicCharacterController;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private float _animationTransitionTime = 0.2f;
    private float _currentState = 0.0f;
    private float _currentVert = 0.0f;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInput>();
    }

    private void setAnimationValues()
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            if (_input.MovementInput != Vector3.zero)
            {
                float newState = Mathf.Clamp01(_motor.Velocity.magnitude / _characterController.MaxStableMoveSpeed);
                _currentState = Mathf.Lerp(_currentState, newState, Time.deltaTime / _animationTransitionTime);
                _currentVert = Mathf.Lerp(_currentVert, 1f, Time.deltaTime / _animationTransitionTime);
            }
            else
            {
                _currentState = Mathf.Lerp(_currentState, 0.0f, Time.deltaTime / _animationTransitionTime);
                _currentVert = Mathf.Lerp(_currentVert, 0.0f, Time.deltaTime / _animationTransitionTime);
            }
            
        }
        if(!_motor.GroundingStatus.IsStableOnGround)
        {
            _currentState = Mathf.Lerp(_currentState, 0.0f, Time.deltaTime / _animationTransitionTime);
            _currentVert = Mathf.Lerp(_currentVert, 0.0f, Time.deltaTime / _animationTransitionTime);
        }
        _animator.SetFloat("State", _currentState);
        _animator.SetFloat("Vert", _currentVert);
    }
    private void Update()
    {
        setAnimationValues();
    }
}