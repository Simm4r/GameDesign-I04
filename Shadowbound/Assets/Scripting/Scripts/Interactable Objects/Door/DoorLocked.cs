using System.Collections;
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
    private bool _hasKey = false;
    private bool _canInteract = false;
    private bool _inCorutine = false;
    [SerializeField] private AudioSource _source;

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
        if (_isLocked && !_doorOpener.IsOpen && !_hasKey)
        {
            NotificationBar.Instance.SetText("The door is locked");
            NotificationBar.Instance.StartBlink();
            return;
        }
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        _inCorutine = true;
        if (_isLocked == true && !_doorOpener.IsOpen)
        {
            _source.Play();
            yield return new WaitUntil(() => !_source.isPlaying);
        }
        _isLocked = false;
        _canInteract = false;
        _doorOpener.StartAnimation();
        _inCorutine = false;
    }

    void Update()
    {
        if (!PlayerInput.Instance.InPossession || NotificationBar.Instance.IsBlinking)
        {
            if (!_doorOpener.IsOpen && _springLock)
                _isLocked = true;
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
                _hasKey = false;
            }

            else if (inventoryItem == _keyItemToOpen)
                _hasKey = true;
            else
            {
                _hasKey = false;
            }

            if (!_doorOpener.IsOpen && _springLock)
                _isLocked = true;
            
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
