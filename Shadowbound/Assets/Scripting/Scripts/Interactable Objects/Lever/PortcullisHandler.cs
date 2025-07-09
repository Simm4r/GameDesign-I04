using UnityEngine;

public class PortcullisHandler : MonoBehaviour
{
    [SerializeField] private bool _isUp = false;
    [SerializeField] private PullLeverHandler _pullLever1;
    [SerializeField] private PullLeverHandler _pullLever2;
    private bool _isActive = false;
    private bool _isLeverSync = false;
    float sum = 0.0f;
    [SerializeField] private bool _startAnimation = false;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float _targetHeight = 3.2f;
    private float speed = 2f;
    [SerializeField] private AudioClip _lift;
    [SerializeField] private AudioClip _lower;
    [SerializeField] private AudioSource _source;
    [SerializeField] private bool _StartAnimation = false;

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
        if (_startAnimation)
        {
            _startAnimation = false;
            StartAnimation();
        }
        
        if (_isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            sum += Time.deltaTime;
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
        sum = 0;
        // Set up for the animation
        if (_isUp)
        {
            _source.resource = _lower;
            _source.Stop();
            _source.Play();
            targetPosition = transform.position + (Vector3.up * (-_targetHeight));
        }
        else
        {
            _source.resource = _lift;
            _source.Stop();
            _source.Play();
            targetPosition = transform.position + (Vector3.up * _targetHeight);
        }
        _isActive = true;
    }
}
