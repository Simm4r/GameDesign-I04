using System;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class LeverSwitchController : MonoBehaviour
{
    private Vector3 _upAngle = new Vector3(-45, 0, 0);
    private Vector3 _downAngle = new Vector3(45, 0, 0);

    [SerializeField] private PullLeverHandler _lever;

    private PortcullisHandler _portcullis;

    [SerializeField] private Quaternion initialRotation;
    [SerializeField] private Quaternion finalRotation;
    [SerializeField] private float duration = 1.0f;
    private bool _isDirectionUp = false;
    [SerializeField] private bool _isActive = false;
    private float time = 0;

    public bool IsActive
    {
        get { return _isActive; }
        // set { _isActive = value; }
    }

    public PortcullisHandler Portcullis
    {
        set{ _portcullis = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Debug.Log($"LeverSwitch initialRotation: {transform.localRotation}");
        _isDirectionUp = _lever.IsUpLever;
        if (_isDirectionUp)
        {
            initialRotation = Quaternion.Euler(_upAngle);
            finalRotation = Quaternion.Euler(_downAngle);
        }
        else
        {
            initialRotation = Quaternion.Euler(_downAngle);
            finalRotation = Quaternion.Euler(_upAngle);
        }
        transform.localRotation = initialRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActive && _portcullis.IsActive && !_portcullis.IsLeverSync)
        {
            // _isActive = true;
            _portcullis.IsLeverSync = true;
            StartAnimation();
        }
        if (_isActive)
            {
                time += Time.deltaTime / duration;
                transform.localRotation = Quaternion.Slerp(initialRotation, finalRotation, time);

                if (transform.localRotation == finalRotation)
                {
                    _isActive = false;
                }
            }

    }

    public void StartAnimation()
    {
        if (_isActive)
        {
            return;
        }
        // same as portcullis?
        _isDirectionUp = _lever.IsUpLever;
        if (_isDirectionUp)
        {
            initialRotation = Quaternion.Euler(_upAngle);
            finalRotation = Quaternion.Euler(_downAngle);
        }
        else
        {
            initialRotation = Quaternion.Euler(_downAngle);
            finalRotation = Quaternion.Euler(_upAngle);
        }
        transform.localRotation = initialRotation;
        time = 0;
        _isActive = true;
    }
}
