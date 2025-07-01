using UnityEngine;

public class StoneAnimator : MonoBehaviour
{
    private Vector3 _minPos;
    private Vector3 _maxPos;
    private Vector3 _actualPos;
    private float _progress = 0.0f;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private bool _startAnimation = false; 
    public enum StoneState
    {
        Up,
        TransitionUp,
        TransitionDown,
        Down
    }
    private StoneState _state = StoneState.Down;
    public StoneState State => _state;
    void Awake()
    {
        _minPos = transform.localPosition;
        _actualPos = _minPos;
        _maxPos = _minPos;
        _maxPos.y = 0.19f;
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
            case StoneState.Down:
                break;
            case StoneState.Up:
                break;
            case StoneState.TransitionUp:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_minPos, _maxPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _maxPos)
                {
                    _state = StoneState.Up;
                    GetComponent<Item>().enabled = true;
                }
                    
                break;
            case StoneState.TransitionDown:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualPos = Vector3.Lerp(_maxPos, _minPos, _progress);
                transform.localPosition = _actualPos;
                if (_actualPos == _minPos)
                    _state = StoneState.Down;
                break;
        }
    }

    public void StartAnimation()
    {
        if (_state == StoneState.TransitionUp || _state == StoneState.TransitionDown)
            return;

        if (_actualPos == _minPos)
            _state = StoneState.TransitionUp;
        else if (_actualPos == _maxPos)
            _state = StoneState.TransitionDown;
        _progress = 0.0f;
    }

}
