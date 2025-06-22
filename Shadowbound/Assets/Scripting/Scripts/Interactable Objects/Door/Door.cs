using System.Runtime.CompilerServices;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private DoorOpener _doorOpener;
    [SerializeField] private InteractionHandler _caller;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        _caller.Player = null;
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
                    _caller.Player = null;
                    _canInteract = false;
                return;
            }
            _caller.Text = _doorOpener.IsOpen ? "Close" : "Open";
            _canInteract = true;
        }

    }
}
