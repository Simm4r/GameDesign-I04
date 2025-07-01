using UnityEngine;

public class AzAnimation : MonoBehaviour
{
    public static AzAnimation Instance { get; private set; }
    public enum AzState
    {
        Hidden,
        Showing,
        OnScreen,
        Hiding
    }
    
    private AzState _state = AzState.Hidden;
    public AzState State
    {
        get => _state;
    }
    private float _progress = 0.0f;
    private Vector3 _minPos;
    [SerializeField] private Vector3 _maxPos;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private ParticleSystem _effects;
    [SerializeField] private bool _startAnimation = false;
    [SerializeField] private MeshRenderer _renderer;
    private Vector3 _actualPos;
    private Quaternion _startRotation;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _minPos = transform.localPosition;
        _actualPos = _minPos;
        _startRotation = transform.localRotation;
    }

    public void Update()
    {
        if (_startAnimation)
        {
            _startAnimation = false;
            StartAnimation();
        }
        switch (_state)
        {
            case AzState.OnScreen:
                Vector3 _player = Player.Instance.gameObject.transform.position + Vector3.up * 0.5f;
                Vector3 lookDirection = _player - transform.position;
                if (lookDirection != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                else
                    transform.localRotation = _startRotation; 
                break;
            case AzState.Showing:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_minPos, _maxPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _maxPos) 
                    _state = AzState.OnScreen;
                break;
            case AzState.Hiding:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_maxPos, _minPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _minPos)
                {
                    _effects.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    _renderer.enabled = false;
                    _state = AzState.Hidden;
                } 
                break;
        }
    }
    public void StartAnimation()
    {
        if (_state == AzState.Showing || _state == AzState.Hiding)
            return;

        if (_state == AzState.Hidden)
        {
            _state = AzState.Showing;
            _renderer.enabled = true;
            _effects.Play(true);
        }

        else
            _state = AzState.Hiding;
        _progress = 0.0f;
    }
}
