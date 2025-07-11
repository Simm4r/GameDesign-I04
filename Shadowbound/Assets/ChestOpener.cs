using UnityEngine;

public class ChestOpener : Interactable
{
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    [SerializeField] private ItemData _key;
    private Inventory _inventory;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private ChestSmall _chest;
    [SerializeField] private AudioSource _source;

    public override void Interact()
    {
        if (_key == null || (_inventory && _inventory.items.Count > 0 && _inventory.items[0].data == _key))
        {
            _chest.StartAnimation();
            return;
        }
        else
        {
            NotificationBar.Instance.SetText("The chest is locked");
            NotificationBar.Instance.StartBlink();
        }
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession
        || _chest.GetState == ChestSmall.ChestSmallState.Opened
        || _chest.GetState == ChestSmall.ChestSmallState.Opening)
        {
            _caller.Player = null;
            _canInteract = false;
            _inventory = null;
            return;
        }
        if (NotificationBar.Instance.IsBlinking)
        {
            _caller.Player = null;
            _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null
        && PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard"))
        {
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _inventory = PossessionHandler.Instance.PossessedEntity.GetComponent<Inventory>();
            _canInteract = true;
            return;
        }
        _canInteract = false;
    }

}
