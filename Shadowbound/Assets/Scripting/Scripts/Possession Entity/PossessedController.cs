using System;
using System.Collections.Generic;
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
    [SerializeField] private float _stableMovementSharpness = 15f;
    [SerializeField] private float _orientationSharpness = 10f;
    [SerializeField] private Vector3 _gravity = new Vector3(0f, -30f, 0f);
    private bool _alreadyPossessed = false;
    private bool _rotationOnly = false;
    private bool _enhanced = false;
    public bool Enhanced
    {
        get => _enhanced;
    }
    private GuardStats _guardStats;
    private void OnEnable() {
        _rotationOnly = false;
        if (gameObject.CompareTag("Possessable_Guard"))
        {
            _guardStats = GetComponent<GuardStats>();
            _guardStats.OnStatusChanged += HandleStateChange;
        }
    }

    private void HandleStateChange(GuardStats.GuardStatus status)
    {
        Debug.Log(status);
        if (status == GuardStats.GuardStatus.Fastened)
        {
            _enhanced = true;
            return;
        }
        _enhanced = false;
    }

    public bool RotationOnly
    {
        get => _rotationOnly;
        set => _rotationOnly = value;
    }
    private List<Collider> _cachedColliders = new();

    public bool AlreadyPossessed
    {
        get { return _alreadyPossessed; }
    }

    public float StableMovementSpeed
    {
        get { return _stableMoveSpeed; }
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
        
    }
    public void SetInputs()
    {
        if (Camera.main == null || Player.Instance.InCutscene || (PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard") && GetComponent<GuardStats>().Status == GuardStats.GuardStatus.Scared))
        {
            _moveInputVector = Vector3.zero;
            return;
        } 
        Transform camera = Camera.main.transform;
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(PlayerInput.Instance.MovementInput.x, 0.0f, PlayerInput.Instance.MovementInput.z), 1.0f);
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(camera.rotation * Vector3.forward, _motor.CharacterUp).normalized;

        if (cameraPlanarDirection.sqrMagnitude == 0.0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(camera.rotation * Vector3.up, _motor.CharacterUp).normalized;
        }

        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, _motor.CharacterUp);
        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = _moveInputVector.normalized;
        if (_enhanced)
        {
            _moveInputVector.Normalize();
            _stableMoveSpeed = _sprintSpeed * 2;
            return;
        }
        if (PlayerInput.Instance.Sprint && !transform.root.CompareTag("Possessable_Object"))
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
        Transform root = transform.root;
        Collider[] colliders = root.GetComponentsInChildren<Collider>(includeInactive: true);

        _cachedColliders.AddRange(colliders);
    }

    public void Update()
    {
       if(!_alreadyPossessed) _alreadyPossessed = true;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (coll.CompareTag("Smoke"))
            return false;
        if (coll.CompareTag("NextAreaWall") && !NotificationBar.Instance.IsBlinking)
        {
            NotificationBar.Instance.SetText("Only Momo can proceed...");
            NotificationBar.Instance.StartBlink();
        }
        if (_cachedColliders.Contains(coll))
            return false;
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
        if (Player.Instance.InCutscene)
            return;
        if (_rotationOnly)
        {
            // Rotazione sul posto in base all'input orizzontale (A/D)
            float horizontalInput = PlayerInput.Instance.MovementInput.x;

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                // Calcola angolo di rotazione
                float rotationSpeed = 120f; // gradi al secondo (modificabile)
                float rotationAmount = horizontalInput * rotationSpeed * deltaTime;

                // Applica la rotazione attorno all’asse Y
                currentRotation *= Quaternion.Euler(0f, rotationAmount, 0f);
            }
            return;
        }
        if (_lookInputVector.sqrMagnitude > 0f && _orientationSharpness > 0.0f)
        {
            Vector3 smoothedLookInputDirection = Vector3.Slerp(_motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-_orientationSharpness * deltaTime)).normalized;
            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_rotationOnly)
        {
            currentVelocity = Vector3.zero;
            return;
        }
        if (Player.Instance.InCutscene)
        {
            currentVelocity = Vector3.zero;
            return;
        }
            
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