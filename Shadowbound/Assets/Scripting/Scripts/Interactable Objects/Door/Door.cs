using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private DoorOpener _doorOpener;
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

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
        {
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
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
