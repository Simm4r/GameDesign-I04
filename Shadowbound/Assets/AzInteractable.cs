using UnityEngine;

public class AzInteractable : Interactable
{
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private DialogueRepeatable _dialogue;

    public override void Interact()
    {
        Player.Instance.InDialogue = true;
        _dialogue.Talk();
        _canInteract = false;
        _caller.Player = null;
    }

    void Update()
    {
        if (_dialogue.InDialogue)
            _dialogue.Talk();
            
        if (PlayerInput.Instance.InPossession)
        {
            _caller.Player = null;
            _canInteract = false;
            if (AzAnimation.Instance.State != AzAnimation.AzState.Hidden)
                AzAnimation.Instance.StartAnimation();
            return;
        }
        _caller.Player = Player.Instance.gameObject;
        if (Vector3.Distance(transform.position, Player.Instance.gameObject.transform.position) <= 4.0f)
        {
            if (AzAnimation.Instance.State != AzAnimation.AzState.OnScreen)
                AzAnimation.Instance.StartAnimation();
        }
        else
        {
            if (AzAnimation.Instance.State != AzAnimation.AzState.Hidden)
                AzAnimation.Instance.StartAnimation();
            _canInteract = false;
            return;
        }
        if (AzAnimation.Instance.State == AzAnimation.AzState.OnScreen && !Player.Instance.InDialogue)
            _canInteract = true;
        else
            _canInteract = false;
    }
}
