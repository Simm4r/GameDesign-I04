using System;
using UnityEngine;

public class PullLeverHandler : MonoBehaviour, IInteractable
{
    [SerializeField] private PortcullisHandler _portcullis;
    [SerializeField] private bool _isUpLever = false;
    [SerializeField] private LeverSwitchController _switchController;
    // private bool isEntityInRange = false;
    // private GameObject entity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool IsUpLever
    {
        get { return _isUpLever; }
        set { _isUpLever = value; }
    }

    void Awake()
    {
        _isUpLever = _portcullis.IsUp;
        _switchController.Portcullis = _portcullis;
        // portcull.SwitchController = _switchController;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // _isUpLever = _portcullis.IsUp;
        // _isUpLever = portcull.IsOpen;
        // if (isEntityInRange && Input.GetKeyDown(KeyCode.E) && !_portcullis.IsActive)
        // {
        //     Debug.Log("Lever Pulled");
        //     _portcullis.StartAnimation();
        //     _switchController.StartAnimation();
        //     // _isUpLever = !_isUpLever;
        //     // _switchController.IsActive = !_switchController.IsActive;
        //     // portcull.IsOpen = !portcull.IsOpen;
        // }
    }

    public void Interact()
    {
        Debug.Log("Lever Interaction started");
        _isUpLever = _portcullis.IsUp;
        if (!_portcullis.IsActive)
        {
            Debug.Log("Lever Pulled");
            _portcullis.StartAnimation();
            _switchController.StartAnimation();
        }
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Possessable_Guard"))
    //     {
    //         isEntityInRange = true;
    //         entity = other.gameObject;
    //     }
    // }

    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Possessable_Guard"))
    //     {
    //         isEntityInRange = false;
    //         entity = null;
    //     }
    // }

}
