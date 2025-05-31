using Unity.VisualScripting;
using UnityEngine;

public class DoorOpener : MonoBehaviour, IInteractable
{
    [SerializeField] private string _tagTrigger = "Player";
    private bool _isEntityInRange = false;
    private GameObject _entity;
    [SerializeField] private bool _isOpen = false;
    [SerializeField] private bool _isAnimationStarted = false;
    [SerializeField] private float _angle = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Quaternion _startRotation;
    private Quaternion _finalRotation;
    private float time = 0;
    [SerializeField] private float _duration = 1.0f;
    void Awake()
    {
        if (_isOpen)
        {
            _startRotation = Quaternion.Euler(Vector3.up * (_angle));
            _finalRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            _startRotation = Quaternion.Euler(Vector3.zero);
            _finalRotation = Quaternion.Euler(Vector3.up * (_angle));
        }

        transform.localRotation = _startRotation;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isAnimationStarted)
        {
            time += Time.deltaTime / _duration;
            transform.localRotation = Quaternion.Slerp(_startRotation, _finalRotation, time);
            if (transform.localRotation == _finalRotation)
            {
                _isOpen = !_isOpen;
                _isAnimationStarted = false;
            }
        }
    }

    public void Interact()
    {
        Debug.Log("Door interaction Started");
        if (!_isAnimationStarted)
        {
            Debug.Log("Opening/Closing the door");
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        if (_isOpen)
        {
            _startRotation = Quaternion.Euler(Vector3.up * (_angle));
            _finalRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            _startRotation = Quaternion.Euler(Vector3.zero);
            _finalRotation = Quaternion.Euler(Vector3.up * (_angle));
        }
        time = 0;
        transform.localRotation = _startRotation;
        _isAnimationStarted = true;
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log($"[DOOR-enter] TAG: {other.tag}");
    //     if (other.CompareTag(_tagTrigger))
    //     {
    //         _isEntityInRange = true;
    //         _entity = other.gameObject;
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     Debug.Log($"[DOOR-exit] TAG: {other.tag}");
    //     if (other.CompareTag(_tagTrigger))
    //     {
    //         _isEntityInRange = false;
    //         _entity = null;
    //     }
    // }
}
