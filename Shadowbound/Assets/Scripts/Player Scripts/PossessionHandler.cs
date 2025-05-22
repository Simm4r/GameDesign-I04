using UnityEngine;



public class PossessionHandler : MonoBehaviour
{
    [SerializeField] private DissolveController _dissolveController;
    [SerializeField] private ParticleSystem _flameRing;
    [SerializeField] private Transform _trialGuard;

    private PlayerInput _input;
    private bool _isPossessing = false;
    private ShadowDamageHandler _shadowHandler;

    public bool IsPossessing {
        get { return _isPossessing; }
        set { _isPossessing = value; }
    }   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _dissolveController = GetComponent<DissolveController>();
        _shadowHandler = GetComponent<ShadowDamageHandler>();
    }
    private void HandlePossessionStart()
    {
        if(!_shadowHandler.CurrentPossessable)
            return;

        if (_input.Possessing)
        {
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
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        CapsuleCollider _collider = GetComponent<CapsuleCollider>();
        ThirdPersonCamera _camera = Camera.main.GetComponent<ThirdPersonCamera>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
                script.enabled = false;
        }
        _collider.enabled = false;
        _camera.player = _trialGuard;
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isPossessing)
            HandlePossessionStart();

        else if (!_dissolveController.IsDissolving)
            HandlePossessionTransition();
    }
}
