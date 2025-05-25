using UnityEngine;

public class LockedDoorOpener : MonoBehaviour
{

    // [SerializeField] private string _tagTrigger = "PossessableGrabberEntity";
    [SerializeField] private ItemData _key;
    [SerializeField] private Inventory _targetInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool _isOpen = false;
    private bool _isAnimationStarted = false;
    private bool _isUnlocked = false;

    private bool _isEntityInRange = false;
    private GameObject _entity;

    [SerializeField] private float _angle = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Quaternion _startRotation;
    private Quaternion _finalRotation;
    private float time = 0;
    [SerializeField] private float _duration = 1.0f;

    void Awake()
    {
        _startRotation = Quaternion.Euler(Vector3.zero);
        _finalRotation = Quaternion.Euler(Vector3.up * (_angle));
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isEntityInRange && Input.GetKeyDown(KeyCode.Q) && !_isAnimationStarted && _targetInventory != null)
        {
            // find element
            bool isContainingKey = _targetInventory.Contains(_key);
            if (isContainingKey)
            {
                StartAnimation();
            }
        }
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[LOCKED-DOOR-enter] TAG: {other.tag}");
        if (other.CompareTag("PossessableGrabberEntity"))
        {
            _isEntityInRange = true;
            _entity = other.gameObject;
            _targetInventory = other.gameObject.GetComponent<Inventory>();

            _targetInventory.PrintInventory();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log($"[LOCKED-DOOR-exit] TAG: {other.tag}");
        if (other.CompareTag("PossessableGrabberEntity"))
        {
            _isEntityInRange = false;
            _entity = null;
            _targetInventory = null;
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

}
