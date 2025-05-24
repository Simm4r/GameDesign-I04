using KinematicCharacterController;
using UnityEngine;

public class PossessedController : MonoBehaviour, ICharacterController
{
    private KinematicCharacterMotor _motor;
    private CapsuleCollider _entityCollider;
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _sprintSpeed = 3.0f;
    [SerializeField] private float _stableMoveSpeed;
    [SerializeField] private float _maxStableMoveSpeed = 5.5f;
    [SerializeField] private float _stableMovementSharpness = 15f;
    [SerializeField] private float _orientationSharpness = 10f;
    [SerializeField] private Vector3 _gravity = new Vector3(0f, -30f, 0f);

    public void AfterCharacterUpdate(float deltaTime)
    {

    }
    public void SetInputs(ref PlayerInput input)
    {
        Transform camera = Camera.main.transform;
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.MovementInput.x, 0.0f, input.MovementInput.z), 1.0f);
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(camera.rotation * Vector3.forward, _motor.CharacterUp).normalized;

        if (cameraPlanarDirection.sqrMagnitude == 0.0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(camera.rotation * Vector3.up, _motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, _motor.CharacterUp);
        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = _moveInputVector.normalized;

        if (input.Sprint)
            _stableMoveSpeed = _sprintSpeed;
        else
            _stableMoveSpeed = _walkSpeed;
    }
    public void Awake()
    {
        _motor = GetComponent<KinematicCharacterMotor>();
        _motor.CharacterController = this;
        _entityCollider = GetComponent<CapsuleCollider>();
        _motor.enabled = false;
        _entityCollider.enabled = false;
        enabled = false;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_lookInputVector.sqrMagnitude > 0f && _orientationSharpness > 0.0f)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(_motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            float currentVelocityMagnitude = currentVelocity.magnitude;
            Vector3 effectiveGroundNormal = _motor.GroundingStatus.GroundNormal;

            currentVelocity = _motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

            Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

            Vector3 tangentMovementVelocity = reorientedInput * _stableMoveSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, tangentMovementVelocity, 1f - Mathf.Exp(-_stableMovementSharpness * deltaTime));
        }
        else
        {
            currentVelocity += _gravity * deltaTime;
        }
    }
}