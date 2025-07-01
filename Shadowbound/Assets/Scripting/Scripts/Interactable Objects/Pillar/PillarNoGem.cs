using UnityEngine;

public class PillarNoGem : Interactable
{
    private bool _canInteract = false;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private ItemData _stoneData;
    [SerializeField] private GameObject _stone;
    [SerializeField] private ItemData _stoneDisabled;
    private ItemData _guardItem;
    private bool _stonePlaced = false;
    private Inventory _guardInventory;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        if (_guardItem == null || _guardItem != _stoneData)
        {
            _caller.Player = null;
            _canInteract = false;
            NotificationBar.Instance.SetText(_guardItem == _stoneDisabled?"The gem must be activated" : "You don't have the right item");
            NotificationBar.Instance.StartBlink();
            return;
        }
        _stonePlaced = true;
        _stone.SetActive(true);
        _guardInventory.RemoveItem(_guardItem);
        _caller.Player = null;
        _canInteract = false;
        _guardItem = null;
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession || _stonePlaced)
        {
            _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
        {
            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _guardInventory = PossessionHandler.Instance.PossessedEntity.GetComponent<Inventory>();
            if (_guardInventory.items.Count > 0 && _guardItem != _guardInventory.items[0].data)
                _guardItem = _guardInventory.items[0].data;
            if (NotificationBar.Instance.IsBlinking)
            {
                _caller.Player = null;
                _canInteract = false;
                return;
            }
            _canInteract = true;
        }

    }
}
