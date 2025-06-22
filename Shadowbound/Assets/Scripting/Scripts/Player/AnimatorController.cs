using KinematicCharacterController;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public static AnimatorController Instance { get; private set; }
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _animationTransitionTime = 0.2f;
    private float _currentState = 0.0f;
    private float _currentVert = 0.0f;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
        _animator = GetComponentInChildren<Animator>();
    }

    private void setAnimationValues()
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            if (PlayerInput.Instance.MovementInput != Vector3.zero)
            {
                float newState = Mathf.Clamp01(Mathf.Pow(_motor.Velocity.magnitude / PlayerController.Instance.SprintSpeed, 2.2f));
                _currentState = Mathf.Lerp(_currentState, newState, Time.deltaTime / _animationTransitionTime);
                float newVert = _motor.Velocity.magnitude > 0.2 ? 1.0f : 0.5f;
                _currentVert = Mathf.Lerp(_currentVert, newVert, Time.deltaTime / _animationTransitionTime);
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