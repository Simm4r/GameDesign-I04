using UnityEngine;

public class LiftPillar : MonoBehaviour
{
    private Vector3 _minPos;
    private Vector3 _maxPos;
    private Vector3 _actualPos;
    private float _progress = 0.0f;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private bool _startAnimation = false; // Per chiamare da editor StartAnimation, per debug
    [SerializeField] private AudioSource _source;
    public enum PillarState
    {
        Up,
        TransitionUp,
        TransitionDown,
        Down
    }
    private PillarState _state = PillarState.Down;
    public PillarState State => _state;
    void Awake()
    {
        _minPos = transform.localPosition;
        _actualPos = _minPos;
        _maxPos = _minPos;
        _maxPos.y = 0;
    }

    void Update()
    {
        if (_startAnimation)
        {
            _startAnimation = false;
            StartAnimation();
        }
        switch (_state)
        {
            case PillarState.Down:
                break;
            case PillarState.Up:
                break;
            case PillarState.TransitionUp:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_minPos, _maxPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _maxPos)
                    _state = PillarState.Up;
                break;
            case PillarState.TransitionDown:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_maxPos, _minPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _minPos)
                    _state = PillarState.Down;
                break;
        }
    }

    public void StartAnimation()
    {
        if (_state == PillarState.TransitionUp || _state == PillarState.TransitionDown)
            return;
        if (_actualPos == _minPos)
            _state = PillarState.TransitionUp;
        else if (_actualPos == _maxPos)
            _state = PillarState.TransitionDown;
        _progress = 0.0f;
        _source.Play();
    }


}
