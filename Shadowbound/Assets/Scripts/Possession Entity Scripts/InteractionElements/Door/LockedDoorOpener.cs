using UnityEngine;

public class LockedDoorOpener : MonoBehaviour, IInteractable
{

    // [SerializeField] private string _tagTrigger = "Possessable_Guard";
    [SerializeField] private ItemData _key;
    [SerializeField] private Inventory _targetInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool _isOpen = false;
    private bool _isContainingKey = false;
    private bool _isAnimationStarted = false;
    [SerializeField] private bool _isUnlocked = false;

    [SerializeField] private float _angle = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Quaternion _startRotation;
    private Quaternion _finalRotation;
    private float time = 0;
    [SerializeField] private float _duration = 1.0f;

    public ItemData Key
    {
        get { return _key; }
        set { _key = value; }
    }

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
        // if (_isEntityInRange && Input.GetKeyDown(KeyCode.E) && !_isAnimationStarted && _targetInventory != null)
        // {
        //     // find element
        //     bool isContainingKey = _targetInventory.Contains(_key);
        //     if (isContainingKey)
        //     {
        //         StartAnimation();
        //     }
        // }
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
        if (_isUnlocked)
        {
            StartAnimation();
        }
        else if (!_isAnimationStarted)
        {
            // bool isContainingKey = _targetInventory.Contains(_key);
            if (_isContainingKey)
            {
                _isUnlocked = true;
                StartAnimation();
            }
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

    public void SetIfHasKey(ItemData data)
    {
        if (data.id == _key.id)
        {
            _isContainingKey = true;
        }   
    }
}
