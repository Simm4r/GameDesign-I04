using System;
using UnityEngine;

public class PullLeverHandler : MonoBehaviour
{
    [SerializeField] private PortcullisHandler _portcull;
    [SerializeField] private bool _isUpLever = false;
    [SerializeField] private LeverSwitchController _switchController;
    private bool isEntityInRange = false;
    private GameObject entity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool IsUpLever
    {
        get { return _isUpLever; }
        set { _isUpLever = value; }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Possessable_Guard"))
        {
            isEntityInRange = true;
            entity = other.gameObject;   
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Possessable_Guard"))
        {
            isEntityInRange = false;
            entity = null;
        }
    }

    void Awake()
    {
        _isUpLever = _portcull.IsUp;
        _switchController.Portcullis = _portcull;
        // portcull.SwitchController = _switchController;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _isUpLever = _portcull.IsUp;
        // _isUpLever = portcull.IsOpen;
        if (isEntityInRange && Input.GetKeyDown(KeyCode.E) && !_portcull.IsActive)
        {
            Debug.Log("Lever Pulled");
            _portcull.StartAnimation();
            _switchController.StartAnimation();
            // _isUpLever = !_isUpLever;
            // _switchController.IsActive = !_switchController.IsActive;
            // portcull.IsOpen = !portcull.IsOpen;
        }
    }
}
