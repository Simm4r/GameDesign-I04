using KinematicCharacterController;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class PossessionHandler : MonoBehaviour
{
    public static PossessionHandler Instance { get; private set; }

    [SerializeField] private ParticleSystem _flameRing;
    [SerializeField] private GameObject _possessedEntity = null;
    [SerializeField] private float _possessionMaxTime = 15f;
    [SerializeField] private float _possessionMaxCooldown = 10f;
    

    private bool _isPossessing = false;
    private KinematicCharacterMotor _possessedMotor;
    private CapsuleCollider _possessedMotorCollider;
    private PossessedController _possessedController;
    private NavMeshAgent _navMeshAgent;
    private Collider _possessedCollider;
    private GuardPatrol _guardPatrol;
    private FleeingEntity _fleeingEntity;
    private Possessable _currentPossessable;
    private float _possessionTime = 0f;
    private float _possessionCooldown = 0f;

    public float PossessionCooldown
    {
        get { return _possessionCooldown; }
    }
    public float PossessionMaxCooldown
    {
        get { return _possessionMaxCooldown; }
    }
    public float PossessionTime
    {
        get { return _possessionTime; }
    }
    public float PossessionMaxTime
    {
        get { return _possessionMaxTime; }
    }
    public bool IsPossessing
    {
        get { return _isPossessing; }
        set { _isPossessing = value; }
    }
    
    public GameObject PossessedEntity
    {
        get { return _possessedEntity; }
    }
    private void SetPossessedEntity()
    {
        _possessedEntity = ShadowHandler.Instance.CurrentPossessable.gameObject.transform.parent ?
            ShadowHandler.Instance.CurrentPossessable.gameObject.transform.parent.gameObject : ShadowHandler.Instance.CurrentPossessable.gameObject;

        if (_possessedEntity.tag == "Possessable_Guard")
        {
            _possessedMotor = _possessedEntity.GetComponent<KinematicCharacterMotor>();
            _possessedMotorCollider = _possessedEntity.GetComponent<CapsuleCollider>();
            _possessedController = _possessedEntity.GetComponent<PossessedController>();
            _navMeshAgent = _possessedEntity.GetComponent<NavMeshAgent>();
            _possessedCollider = ShadowHandler.Instance.CurrentPossessable.gameObject.GetComponent<CapsuleCollider>();
            _guardPatrol = _possessedEntity.GetComponent<GuardPatrol>();

            Canvas[] marks = _possessedEntity.GetComponentsInChildren<Canvas>();

            foreach (Canvas mark in marks)
            {
                Debug.Log(mark.gameObject);
                if (mark.gameObject.activeSelf && !mark.gameObject.CompareTag("Inventory"))
                    mark.gameObject.SetActive(false);
            }
            
            InventoryUI inventoryUI = _possessedEntity.GetComponentInChildren<InventoryUI>();
            if (inventoryUI != null && !_possessedController.AlreadyPossessed)
                {
                    inventoryUI.enableInventoryUI();
                }
            if (inventoryUI != null && _possessedController.AlreadyPossessed)
            {
                inventoryUI.HideInventoryUI();
            }
        }
        else if (_possessedEntity.tag == "Possessable_Object")
        {
            _possessedMotor = _possessedEntity.GetComponent<KinematicCharacterMotor>();
            _possessedMotorCollider = _possessedEntity.GetComponent<CapsuleCollider>();
            _possessedController = _possessedEntity.GetComponent<PossessedController>();
            _possessedCollider = ShadowHandler.Instance.CurrentPossessable.gameObject.GetComponentInChildren<MeshCollider>();
        }
        else if (_possessedEntity.tag == "Possessable_Animal")
        {
            _possessedMotor = _possessedEntity.GetComponent<KinematicCharacterMotor>();
            _possessedMotorCollider = _possessedEntity.GetComponent<CapsuleCollider>();
            _possessedController = _possessedEntity.GetComponent<PossessedController>();
            _navMeshAgent = _possessedEntity.GetComponent<NavMeshAgent>();
            _possessedCollider = ShadowHandler.Instance.CurrentPossessable.gameObject.GetComponent<CapsuleCollider>();
            _fleeingEntity = _possessedEntity.GetComponent<FleeingEntity>();
        }
        _currentPossessable = ShadowHandler.Instance.CurrentPossessable;

    }
    private void UnsetPossessedEntity()
    {
        InventoryUI inventory = _possessedEntity.GetComponentInChildren<InventoryUI>();
        if (inventory != null)
        {
            inventory.ShowInventoryUI();
        }
        _possessedEntity = null;
        _possessedMotor = null;
        _possessedMotorCollider = null;
        _possessedController = null;
        _navMeshAgent = null;
        _possessedCollider = null;
        _guardPatrol = null;
        _fleeingEntity = null;
        _currentPossessable = null;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;     
    }
    private void HandlePossessionStart()
    {
        if (_possessionCooldown > 0.0f)
            return;

        if (!ShadowHandler.Instance.CurrentPossessable)
                return;

        if (ShadowHandler.Instance.CurrentPossessable.GetComponentInParent<EntityStats>().EntityLevel > GetComponent<PlayerStats>().PossessionLevel)
            return;
        if (PlayerInput.Instance.Possessing)
            {
                Healthbar.Instance.gameObject.SetActive(false);
                //Setto la possessable entity target
                SetPossessedEntity();

                _isPossessing = true;
                if (DissolveController.Instance != null)
                    DissolveController.Instance.StartDissolve();

                if (_flameRing != null)
                {
                    _flameRing.Clear();
                    _flameRing.Play();

                }
            }
    }

    private Vector3 ComputeSafeDirection(Vector3 fromPosition)
    {
        Vector3[] directions = new Vector3[3];
        directions[0] = -_possessedEntity.transform.forward;
        directions[1] = (-_possessedEntity.transform.forward + _possessedEntity.transform.right).normalized;
        directions[2] = (-_possessedEntity.transform.forward - _possessedEntity.transform.right).normalized;

        float maxDistance = 1.5f;
        RaycastHit[] hits;
        Vector3 bestDirection = Vector3.zero;
        float furthestHit = 0f;

        Collider[] ignoredColliders = new Collider[] {
            GetComponent<Collider>(),
            _possessedCollider,
            _possessedMotorCollider
        };

        foreach (Vector3 dir in directions)
        {
            bool blocked = false;
            float minHitDistance = maxDistance;

            hits = Physics.RaycastAll(fromPosition, dir, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if (System.Array.Exists(ignoredColliders, col => col == hit.collider))
                    continue;


                blocked = true;
                if (hit.distance < minHitDistance)
                    minHitDistance = hit.distance;
            }

            if (!blocked)
            {

                return dir;
            }
            else if (minHitDistance > furthestHit)
            {
                furthestHit = minHitDistance;
                bestDirection = dir;
            }
        }

        return bestDirection;
    }

    private void HandlePossessionTransition()
    {

        _currentPossessable.HidePossessableCue();
        _possessedCollider.enabled = false;

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

        if (_possessedEntity.tag != "Possessable_Object" && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;

            if (_possessedEntity.tag == "Possessable_Guard")
                _guardPatrol.enabled = false;
            else if (_possessedEntity.tag == "Possessable_Animal")
                _fleeingEntity.enabled = false;
        }

        _possessedMotorCollider.enabled = true;
        _possessedMotor.SetPositionAndRotation(currentPositionAndRotation.position, currentPositionAndRotation.rotation);
        _possessedController.enabled = true;
        _possessedMotor.enabled = true;

        _camera.player = _possessedEntity.transform;

        PlayerInput.Instance.InPossession = true;
        _possessedEntity.GetComponentInChildren<EyeParticlesHandler>().LitEyes();
        _possessionCooldown = _possessionMaxCooldown;
    }

    private void HandlePossessionEnd()
    {

        _possessedMotor.enabled = false;
        _possessedMotorCollider.enabled = false;
        _possessedController.enabled = false;
        _possessedCollider.enabled = true;

        if (_possessedEntity.tag == "Possessable_Guard")
        {
            _navMeshAgent.enabled = true;
            _guardPatrol.enabled = true;
        }
        else if (_possessedEntity.tag == "Possessable_Animal")
        {
            _navMeshAgent.enabled = true;
            _fleeingEntity.enabled = true;
        }

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
                Vector3 correctedPosition = new Vector3(currentPositionAndRotation.position.x, _possessedMotor.GroundingStatus.GroundPoint.y + _collider.radius, currentPositionAndRotation.position.z);
                Vector3 safeDirection = ComputeSafeDirection(correctedPosition);
                motor.SetPositionAndRotation(correctedPosition, currentPositionAndRotation.rotation);
                motor.MoveCharacter(correctedPosition + safeDirection * 1.5f);
            }

        }
        _collider.enabled = true;
        _camera.player = gameObject.transform;
        UndissolveController.Instance.StartUndissolve();
        if (_flameRing != null)
        {
            _flameRing.Clear();
            _flameRing.Play();
        }
        _isPossessing = false;
        PlayerInput.Instance.InPossession = false;
        
        _possessedEntity.GetComponentInChildren<EyeParticlesHandler>().UnlitEyes();
        UnsetPossessedEntity();
        GetComponent<PlayerStats>().ResetPlayer();
        _possessionTime = 0.0f;
        Healthbar.Instance.gameObject.SetActive(true);
    }



    // Update is called once per frame
    void Update()
    {
        if (!_isPossessing)
            HandlePossessionStart();

        else if (!DissolveController.Instance.IsDissolving && !PlayerInput.Instance.InPossession)
            HandlePossessionTransition();

        if (_possessionTime == _possessionMaxTime || PlayerInput.Instance.QuitPossession)
            HandlePossessionEnd();

        if (PlayerInput.Instance.InPossession)
        {
            _possessedController.SetInputs();
            _possessionTime += Time.deltaTime;
            _possessionTime = Mathf.Clamp(_possessionTime, 0.0f, _possessionMaxTime);
        }
        else if (Time.timeScale == 1.0f && _possessionCooldown > 0.0f)
        {
            _possessionCooldown -= Time.deltaTime;
            _possessionCooldown = Mathf.Clamp(_possessionCooldown, 0.0f, _possessionMaxCooldown);
        }
    }
}
