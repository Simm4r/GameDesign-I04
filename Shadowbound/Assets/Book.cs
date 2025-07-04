using Unity.VisualScripting;
using UnityEngine;

public class Book : MonoBehaviour
{
    public enum BookState
    {
        Steady,
        Pulling,
        Pulled
    }
    private BookState _state = BookState.Steady;
    private Quaternion _startRotation;
    private Quaternion _targetRotation = Quaternion.Euler(20, 270, 0);
    private Quaternion _currentRotation;
    private float _progress = 0.0f;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private HiddenWallLift _wall;
    [SerializeField] private bool _startAnimation = false;
    void Awake()
    {
        _startRotation = transform.localRotation;
        _currentRotation = _startRotation;
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
            case BookState.Pulling:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _currentRotation = Quaternion.Lerp(_startRotation, _targetRotation, _progress);
                transform.localRotation = _currentRotation;
                if (_currentRotation == _targetRotation)
                {
                    _state = BookState.Pulled;
                    _wall.StartAnimation();   
                }
                break;
        }
    }

    public void StartAnimation()
    {
        if (_state != BookState.Steady)
            return;
        _progress = 0;
        _state = BookState.Pulling;
    }
}
