using UnityEngine;

public class RepeatableRead : Interactable
{
    private bool _canInteract = false;
    [SerializeField] private InteractionHandler _caller;
    [TextArea(5, 10)]
    [SerializeField] private string _content;
    private bool _readingThis = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        _caller.Player = null;
        _canInteract = false;
        Player.Instance.Reading = true;
        _readingThis = true;
        ScrollHUDHandler.Instance.SetText(_content);
        ScrollHUDHandler.Instance.Show();
    }

    void Update()
    {
        if (PlayerInput.Instance.InPossession)
        {
            _caller.Player = null;
            _canInteract = false;
            return;
        }

        _caller.Player = Player.Instance.gameObject;

        if (ScrollHUDHandler.Instance.GetActualState() == "Idle")
        {
            _canInteract = true;
            Player.Instance.Reading = false;
        }

    }
}
