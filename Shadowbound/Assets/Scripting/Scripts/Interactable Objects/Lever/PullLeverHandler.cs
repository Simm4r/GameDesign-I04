using System;
using UnityEngine;

public class PullLeverHandler : Interactable
{
    [SerializeField] private PortcullisHandler _portcullis;
    [SerializeField] private bool _isUpLever = false;
    [SerializeField] private LeverSwitchController _switchController;
    [SerializeField] private InteractionHandler _caller;

    private bool _canInteract = false;

    public PortcullisHandler Portcullis
    {
        get { return _portcullis; }
    }

    public bool IsUpLever
    {
        get { return _isUpLever; }
        set { _isUpLever = value; }
    }

    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }


    void Awake()
    {
        _isUpLever = _portcullis.IsUp;
        _switchController.Portcullis = _portcullis;
    }

    public override void Interact()
    {
        _caller.Player = null;
        _canInteract = false;
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

    void Update()
    {
        if (!PlayerInput.Instance.InPossession)
        {
                _caller.Player = null;
                _canInteract = false;
            return;
        }

        if (_portcullis.IsUp != _isUpLever)
        {
            _isUpLever = _portcullis.IsUp;
            _caller.Text = _isUpLever ? "Pull down" : "Pull up";
        }
            
        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
            {
                _caller.Player = PossessionHandler.Instance.PossessedEntity;
                if (_portcullis.IsActive)
                {
                    _caller.Player = null;
                    _canInteract = false;
                    return;
                }

                _canInteract = true;
            }

    }
}
