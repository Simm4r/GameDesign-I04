using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorLocked : Interactable
{
    [SerializeField] private DoorOpener _doorOpener;
    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private ItemData _keyItemToOpen;
    private GameObject _possessedEntity;
    [SerializeField] private bool _isLocked = false;
    [SerializeField] private bool _springLock = false;
    private bool _canInteract = false;

    public bool IsLocked
    {
        get => _isLocked;
        set => _isLocked = value;
    }
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        Debug.Log("IsLocked: " + _isLocked + " IsOpen: " + _doorOpener.IsOpen);
        if (_isLocked && !_doorOpener.IsOpen)
        {
            NotificationBar.Instance.SetText("The door is locked");
            NotificationBar.Instance.StartBlink();
            return;
        }
        _canInteract = false;
        _doorOpener.StartAnimation();
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession || NotificationBar.Instance.IsBlinking)
        {
            _canInteract = false;
            return;
        }

        if (PossessionHandler.Instance.PossessedEntity != null && PossessionHandler.Instance.PossessedEntity.tag == "Possessable_Guard")
        {
            _possessedEntity = PossessionHandler.Instance.PossessedEntity;
            _caller.Player = _possessedEntity;

            Inventory inventory = _possessedEntity.GetComponent<Inventory>();

            ItemData inventoryItem = inventory.items.Count == 0 ? null : inventory.items[0].data;

            if (inventoryItem == null)
            {
                if (_springLock)
                    _isLocked = true;
            }

            else if (inventoryItem == _keyItemToOpen)
                _isLocked = false;
            else
            {
                if (_springLock)
                    _isLocked = true;
            }
                
            
            if (_doorOpener.IsAnimationStarted)
            {
                if (_canInteract)
                    _canInteract = false;
                return;
            }

            _caller.Text = _doorOpener.IsOpen ? "Close" : "Open";
            _canInteract = true;
        }

    }
}
