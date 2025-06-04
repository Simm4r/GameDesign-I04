using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private DoorOpener _doorOpener;
    [SerializeField] private PossessionHandler _possessionHandler;
    [SerializeField] private InteractionHandler _caller;
    private bool _canInteract = true;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        _canInteract = false;
        _doorOpener.StartAnimation();
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;
            return;
        }

        if (_possessionHandler.PossessedEntity != null && _possessionHandler.PossessedEntity.tag == "Possessable_Guard")
        {
            _caller.Player = _possessionHandler.PossessedEntity;
            if (_doorOpener.IsAnimationStarted)
            {
                if(_canInteract)
                    _canInteract = false;
                return;
            }

            _canInteract = true;
        }

    }
}
