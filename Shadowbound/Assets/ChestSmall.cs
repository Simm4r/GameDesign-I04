using System;
using UnityEngine;

public class ChestSmall : MonoBehaviour
{
    public enum ChestSmallState
    {
        Closed,
        Opened,
        Opening,
        Closing
    }
    private ChestSmallState _state = ChestSmallState.Closed;
    private Quaternion _startRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    [SerializeField] private Quaternion _targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
    private Quaternion _actualRotation;
    private float _progress = 0.0f;
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private bool _startAnimation = false;
    [SerializeField] private GameObject _lid;
    [SerializeField] private AudioSource _source;

    void Awake()
    {
        _actualRotation = _lid.transform.localRotation;
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
            case ChestSmallState.Opening:
                _progress = Mathf.Clamp01(_progress + _speed * Time.deltaTime);
                _actualRotation = Quaternion.Lerp(_startRotation, _targetRotation, _progress);
                _lid.transform.localRotation = _actualRotation;
                if (_progress == 1.0f)
                    _state = ChestSmallState.Opened;
                break;
            default:
                return;
        }
    }

    public void StartAnimation()
    {
        if (_state == ChestSmallState.Opening || _state == ChestSmallState.Opened || _state == ChestSmallState.Closing)
            return;
        _source.Play();
        _state = ChestSmallState.Opening;
        _progress = 0.0f;
    }

    public ChestSmallState GetState => _state;
}
