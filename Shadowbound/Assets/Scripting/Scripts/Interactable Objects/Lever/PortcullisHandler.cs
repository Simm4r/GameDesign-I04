using UnityEngine;

public class PortcullisHandler : MonoBehaviour
{
    [SerializeField] private bool _isUp = false;
    [SerializeField] private PullLeverHandler _pullLever1;
    [SerializeField] private PullLeverHandler _pullLever2;
    private bool _isActive = false;
    private bool _isLeverSync = false;

    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float _targetHeight = 3.2f;
    private float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool IsUp
    {
        get { return _isUp; }
        set { _isUp = value; }
    }

    public bool IsActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }

    public bool IsLeverSync
    {
        get{ return _isLeverSync; }
        set { _isLeverSync = value; }
    }

    void Awake()
    {
        // _isOpen = 
        Vector3 startPosition = transform.position;

        if (_isUp)
        {
            transform.position += (Vector3.up * _targetHeight);
            targetPosition = transform.position + (Vector3.up * (-_targetHeight));
        }
        else
        {
            targetPosition = transform.position + (Vector3.up * _targetHeight);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (transform.position == targetPosition)
            {
                _pullLever1.CanInteract = true;
                _pullLever2.CanInteract = true;
                _isUp = !_isUp;
                _isActive = false;
                _isLeverSync = false;
            }
        }
    }



    public void StartAnimation()
    {
        if (_isActive)
        {
            return;
        }

        // Set up for the animation
        if (_isUp)
        {
            targetPosition = transform.position + (Vector3.up * (-_targetHeight));
        }
        else
        {
            targetPosition = transform.position + (Vector3.up * _targetHeight);
        }
        _isActive = true;
    }
}
