using System;
using UnityEngine;

public class DoorLargeOpener : MonoBehaviour
{
    public enum DoorState
    {
        Close,
        Opening,
        Open
    }

    private DoorState _state = DoorState.Close;
    [SerializeField] private GameObject _doorLeft;
    [SerializeField] private GameObject _doorRight;
    private Quaternion _startAngle = Quaternion.Euler(0f, 0f, 0f);
    private Quaternion _targetAngleR = Quaternion.Euler(0f, 90f, 0f);
    private Quaternion _targetAngleL = Quaternion.Euler(0f, -90f, 0f);
    private float _progress = 0.0f;
    [SerializeField] private float _speed = 1.0f;
    private Quaternion _actualRotationR;
    private Quaternion _actualRotationL;
    [SerializeField] private bool _startAnimation = false;
    void Awake()
    {
        _actualRotationR = _startAngle;
        _actualRotationL = _startAngle;
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
            case DoorState.Opening:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualRotationR = Quaternion.Lerp(_startAngle, _targetAngleR, _progress);
                _actualRotationL = Quaternion.Lerp(_startAngle, _targetAngleL, _progress);
                _doorRight.transform.localRotation = _actualRotationR;
                _doorLeft.transform.localRotation = _actualRotationL;
                if (_progress == 1.0f)
                    _state = DoorState.Open;
                break;
            default:
                return;
        }
    }

    public void StartAnimation()
    {
        if (_state != DoorState.Close)
            return;
        _state = DoorState.Opening;
        _progress = 0.0f;
    }

    public DoorState GetState()
    {
        return _state;
    }
}
