using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;



public class PossessionHandler : MonoBehaviour
{
    [SerializeField] private DissolveController _dissolveController;
    [SerializeField] private UndissolveController _undissolveController;
    [SerializeField] private ParticleSystem _flameRing;
    [SerializeField] private GameObject _possessedEntity = null;
    [SerializeField] private float _possessionMaxTime = 15f;
    [SerializeField] private GuardAnimatorController _guardAnimationController;
    

    [SerializeField] private PlayerInput _input;
    private bool _isPossessing = false;
    private ShadowDamageHandler _shadowHandler;
    private KinematicCharacterMotor _possessedMotor;
    private CapsuleCollider _possessedPlayebleCollider;
    private PossessedController _possessedController;
    private NavMeshAgent _navMeshAgent;
    private CapsuleCollider _navMeshCollider;
    private GuardPatrol _guardPatrol;
    private float _possessionTime = 0f;

    public bool IsPossessing
    {
        get { return _isPossessing; }
        set { _isPossessing = value; }
    }

    private void SetPossessedEntity()
    {
        _possessedEntity = _shadowHandler.CurrentPossessable.gameObject.transform.parent.gameObject;
        _possessedMotor = _possessedEntity.GetComponent<KinematicCharacterMotor>();
        _possessedPlayebleCollider = _possessedEntity.GetComponent<CapsuleCollider>();
        _possessedController = _possessedEntity.GetComponent<PossessedController>();
        _navMeshAgent = _possessedEntity.GetComponent<NavMeshAgent>();
        _navMeshCollider = _shadowHandler.CurrentPossessable.gameObject.GetComponent<CapsuleCollider>();
        _guardPatrol = _possessedEntity.GetComponent<GuardPatrol>();
        _guardAnimationController = _possessedEntity.GetComponent<GuardAnimatorController>();
    }
    private void UnsetPossessedEntity()
    {
        _possessedEntity = null;
        _possessedMotor = null;
        _possessedPlayebleCollider = null;
        _possessedController = null;
        _navMeshAgent = null;
        _navMeshCollider = null;
        _guardPatrol = null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _dissolveController = GetComponent<DissolveController>();
        _undissolveController = GetComponent<UndissolveController>();
        _shadowHandler = GetComponent<ShadowDamageHandler>();
        
    }
    private void HandlePossessionStart()
    {
        if(!_shadowHandler.CurrentPossessable)
            return;
  
        if (_input.Possessing)
        {
            //Setto la possessable entity target
            SetPossessedEntity();

            _isPossessing = true;
            if (_dissolveController != null)
                _dissolveController.StartDissolve();

            if (_flameRing != null)
            {
                _flameRing.Clear();
                _flameRing.Play();
            }
        }
    }
    private void HandlePossessionTransition()
    {
        _shadowHandler.CurrentPossessable.HidePossessableCue();
        _navMeshCollider.enabled = false;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        CapsuleCollider _collider = GetComponent<CapsuleCollider>();
        ThirdPersonCamera _camera = Camera.main.GetComponent<ThirdPersonCamera>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && !(script is PlayerInput) && !(script is DissolveController) && !(script is UndissolveController))
                script.enabled = false;
        }
        _collider.enabled = false;
        Transform currentPositionAndRotation = _possessedEntity.transform;

        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _guardPatrol.enabled = false;
            _navMeshAgent.enabled = false;
        }

        _possessedPlayebleCollider.enabled = true;
        _possessedMotor.SetPositionAndRotation(currentPositionAndRotation.position, currentPositionAndRotation.rotation);
        _possessedController.enabled = true;
        _possessedMotor.enabled = true;
        _guardAnimationController.enabled = true;
        _camera.player = _possessedEntity.transform;

        _input.InPossession = true;
    }

    private void HandlePossessionEnd()
    {
        _possessionTime = 0.0f;
        _possessedMotor.enabled = false;
        _possessedPlayebleCollider.enabled = false;
        _possessedController.enabled = false;
        _guardAnimationController.enabled = false;

        _guardPatrol.enabled = true;
        _navMeshCollider.enabled = true;
        _navMeshAgent.enabled = true;
        Transform currentPositionAndRotation = _possessedEntity.transform;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        CapsuleCollider _collider = GetComponent<CapsuleCollider>();
        ThirdPersonCamera _camera = Camera.main.GetComponent<ThirdPersonCamera>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && !(script is PlayerInput) && !(script is PlayerInput) && !(script is DissolveController) && !(script is UndissolveController))
                script.enabled = true;
            if (script is KinematicCharacterMotor motor)
            {
                Vector3 correctedPosition = new Vector3(currentPositionAndRotation.position.x, motor.GroundingStatus.GroundPoint.y + _collider.radius, currentPositionAndRotation.position.z);
                motor.SetPositionAndRotation(correctedPosition - _possessedEntity.transform.forward * 1.5f, currentPositionAndRotation.rotation);
                motor.ForceUnground();
            }

        }
        _collider.enabled = true;
        _shadowHandler.CurrentPossessable.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        _camera.player = gameObject.transform;
        _undissolveController.StartUndissolve();
        if (_flameRing != null)
        {
            _flameRing.Clear();
            _flameRing.Play();
        }
        _isPossessing = false;
        _input.InPossession = false;
        UnsetPossessedEntity();
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isPossessing)
            HandlePossessionStart();

        else if (!_dissolveController.IsDissolving && !_input.InPossession)
            HandlePossessionTransition();

        if (_possessionTime == _possessionMaxTime)
            HandlePossessionEnd();

        if (_input.InPossession)
        {
            _possessedController.SetInputs(ref _input);
            _possessionTime += Time.deltaTime;
            _possessionTime = Mathf.Clamp(_possessionTime, 0.0f, _possessionMaxTime);
        }
    }
}
