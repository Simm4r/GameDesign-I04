using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject _choosingSphere;
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
    private bool _choosingPosition = false;
    private bool _quitImmediate = false;

    public bool QuitImmediate
    {
        get => _quitImmediate;
        set => _quitImmediate = value;
    }

    public bool ChoosingPosition
    {
        get => _choosingPosition;
        set => _choosingPosition = value;
    }

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
            ShadowHandler.Instance.CurrentPossessable.gameObject.transform.root.gameObject : ShadowHandler.Instance.CurrentPossessable.gameObject;

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
            Destroy(this);
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
        float maxDistance = 0.7f;
        Vector3 bestDirection = Vector3.zero;
        float furthestHit = 0f;

        Collider[] ignoredColliders = new Collider[]
        {
            GetComponent<Collider>(),
            _possessedCollider,
            _possessedMotorCollider
        };

        // Costruzione delle direzioni circolari (intorno al posseduto)
        Vector3 forward = _possessedEntity.transform.forward;
        Vector3 right = _possessedEntity.transform.right;

        List<Vector3> directions = new List<Vector3>();

        int steps = 16;
        for (int i = 0; i < steps; i++)
        {
            float angle = (360f / steps) * i;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            directions.Add(rot * -forward); // Direzioni distribuite in cerchio
        }

        foreach (Vector3 dir in directions)
        {
            bool blocked = false;
            float minHitDistance = maxDistance;

            RaycastHit[] hits = Physics.RaycastAll(fromPosition, dir, maxDistance);
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
                return dir; // prima direzione libera
            }
            else if (minHitDistance > furthestHit)
            {
                furthestHit = minHitDistance;
                bestDirection = dir; // direzione meno ostruita
            }
        }

        return bestDirection;
    }


    private void HandlePossessionTransition()
    {

        _currentPossessable.HidePossessableCue();

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
        _quitImmediate = false;
        _possessedMotor.enabled = false;
        _possessedMotorCollider.enabled = false;
        _possessedController.enabled = false;

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

        //Transform currentPositionAndRotation = _possessedEntity.transform;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        //CapsuleCollider _collider = GetComponent<CapsuleCollider>();
        ThirdPersonCamera _camera = Camera.main.GetComponent<ThirdPersonCamera>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && !(script is PlayerInput) && !(script is PlayerInput) && !(script is DissolveController) && !(script is UndissolveController && !(script is KinematicCharacterMotor)))
                script.enabled = true;
            //if (script is KinematicCharacterMotor motor)
            //{
            //    Vector3 correctedPosition = new Vector3(currentPositionAndRotation.position.x, _possessedMotor.GroundingStatus.GroundPoint.y + _collider.radius, currentPositionAndRotation.position.z);
            //    Time.timeScale = 0.0f;
            //    _choosingPosition = true;
            //Vector3 safeDirection = ComputeSafeDirection(correctedPosition);
            //motor.MoveCharacter(correctedPosition + safeDirection * 1.5f);
            //}

        }
        Vector3 direction = ComputeSafeDirection(_possessedEntity.transform.position + Vector3.up * 0.15f);
        _choosingSphere.transform.position = _possessedEntity.transform.position  + direction * 0.7f + Vector3.up * 0.15f;
        Time.timeScale = 0.0f;
        _choosingPosition = true;
        _choosingSphere.SetActive(true);
        _camera.player = _choosingSphere.transform;
        //_collider.enabled = true;
        //_camera.player = gameObject.transform;
        //UndissolveController.Instance.StartUndissolve();
        //if (_flameRing != null)
        //{
        //    _flameRing.Clear();
        //    _flameRing.Play();
        //}
        //_isPossessing = false;
        //PlayerInput.Instance.InPossession = false;

        //_possessedEntity.GetComponentInChildren<EyeParticlesHandler>().UnlitEyes();
        //UnsetPossessedEntity();
        //GetComponent<PlayerStats>().ResetPlayer();
        //_possessionTime = 0.0f;
        //Healthbar.Instance.gameObject.SetActive(true);
    }



    // Update is called once per frame
    void Update()
    {
        if (PauseHandler.Instance.InPause || Player.Instance.InCutscene)
            return;
        if (_choosingPosition)
        {
            HandleExitPosition();
            return;
        }
        if (!_isPossessing)
            HandlePossessionStart();

        else if (!DissolveController.Instance.IsDissolving && !PlayerInput.Instance.InPossession)
            HandlePossessionTransition();

        if (_possessionTime == _possessionMaxTime || PlayerInput.Instance.QuitPossession || _quitImmediate)
            HandlePossessionEnd();

        if (PlayerInput.Instance.InPossession && !_choosingPosition)
        {
            
            _possessedController.SetInputs();
            if (PlayerInput.Instance.Interact && _possessedEntity.GetComponent<MultiTag>()?.HasTag("Mirror") == true)
            {
                _possessedController.RotationOnly = !_possessedController.RotationOnly;
            }
            if (!(PlayerStats.Instance.PossessionLevel >= 3 && _possessedMotor.Velocity == Vector3.zero))
            {
                _possessionTime += Time.deltaTime;
                _possessionTime = Mathf.Clamp(_possessionTime, 0.0f, _possessionMaxTime);
            }
        }
        else if (Time.timeScale == 1.0f && _possessionCooldown > 0.0f)
        {
            _possessionCooldown -= Time.deltaTime;
            _possessionCooldown = Mathf.Clamp(_possessionCooldown, 0.0f, _possessionMaxCooldown);
        }
    }

    private void HandleExitPosition()
    {

        if (!PlayerInput.Instance.ConfirmPosition || UndissolveController.Instance.IsUndissolving)
            return;

        Vector3 correctedPosition = _choosingSphere.transform.position;

        CapsuleCollider _collider = GetComponent<CapsuleCollider>();
        correctedPosition.y += _collider.radius;
        ThirdPersonCamera _camera = Camera.main.GetComponent<ThirdPersonCamera>();
        KinematicCharacterMotor motor = GetComponent<KinematicCharacterMotor>();
        motor.enabled = true;
        
        motor.SetPositionAndRotation(PossessedEntity.transform.position + correctedPosition.normalized * 0.5f, PossessedEntity.transform.rotation);
        motor.SetPosition(correctedPosition);
        _collider.enabled = true;
        _camera.player = gameObject.transform;
        
        _choosingSphere.SetActive(false);
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
}
