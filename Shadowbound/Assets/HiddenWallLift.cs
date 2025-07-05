using System;
using UnityEngine;

public class HiddenWallLift : MonoBehaviour
{
    public enum WallState
    {
        Down,
        Lifting,
        Up
    }

    private WallState _state = WallState.Down;
    public WallState State
    {
        get => _state;
    }
    private Vector3 _currentPos;
    private Vector3 _minPos;
    [SerializeField] private Vector3 _maxPos;
    private float _progress;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private bool _startAnimation = false;

    void Awake()
    {
        _minPos = transform.localPosition;
        _currentPos = _minPos;
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
            case WallState.Lifting:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _currentPos = Vector3.Lerp(_minPos, _maxPos, _progress);
                transform.localPosition = _currentPos;
                if (_currentPos == _maxPos)
                    _state = WallState.Up;
                break;
        }
    }

    public void StartAnimation()
    {
        if (_state != WallState.Down)
            return;
        _state = WallState.Lifting;
    }
}
