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
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _animator = GetComponentInChildren<Animator>();
    }

    private void setAnimationValues()
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            float currentSpeed = _motor.Velocity.magnitude;

            float walkSpeed = 1.25f;
            float sprintThreshold = 1.5f;

            float normalizedSpeed = currentSpeed / walkSpeed;
            normalizedSpeed = Mathf.Clamp(normalizedSpeed, 0f, 2f);

            float visualVert = normalizedSpeed;
            if (visualVert > 0.05f && visualVert < 0.4f)
                visualVert = 0.4f;

            float vertTransitionTime = 0.1f;
            _currentVert = Mathf.Lerp(_currentVert, Mathf.Clamp01(visualVert), Time.deltaTime / vertTransitionTime);

            float targetState = currentSpeed > sprintThreshold ? 1f : 0f;
            _currentState = Mathf.Lerp(_currentState, targetState, Time.deltaTime / _animationTransitionTime);
        }
        else
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