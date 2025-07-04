using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    public static HUDHandler Instance { get; private set; }
    [SerializeField] private GameObject _textBox;
    [SerializeField] private GameObject _cooldownBar;
    [SerializeField] private GameObject _scroll;
    [SerializeField] private GameObject _interactionBar;
    [SerializeField] private GameObject _notificationBar;
    [SerializeField] private GameObject _tutorial;
    private bool _isRotating = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    void Update()
    {
        if (PauseHandler.Instance.InPause || Player.Instance.InCutscene)
        {
            if(PauseHandler.Instance.InPause)
                _textBox.SetActive(false);
            _cooldownBar.SetActive(false);
            _scroll.SetActive(false);
            _interactionBar.SetActive(false);
            _notificationBar.SetActive(false);
            _tutorial.SetActive(false);
            SmallNotificationBarHandler.Instance.HideBar();
            AuxiliaryBarHandler.Instance.HideBar();
            return;
        }
        _tutorial.SetActive(true);
        _scroll.SetActive(true);
        if (PossessionHandler.Instance.ChoosingPosition)
        {
            _interactionBar.SetActive(false);
            SmallNotificationBarHandler.Instance.ShowBar();
            InventoryHUD.Instance.HideInventoryHUD();
        }
        else
            _interactionBar.SetActive(true);

        if (Player.Instance.InDialogue || Player.Instance.Reading)
        {
            _textBox.SetActive(Player.Instance.InDialogue);
            _cooldownBar.SetActive(false);
            _interactionBar.SetActive(false);
            _notificationBar.SetActive(false);
            SmallNotificationBarHandler.Instance.HideBar();
        }

        else
        {
            _textBox.SetActive(false);
            _cooldownBar.SetActive(true);
            _interactionBar.SetActive(true);
            _notificationBar.SetActive(true);
        }

        if (PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard") && !PossessionHandler.Instance.ChoosingPosition)
        {
            Inventory inventory = PossessionHandler.Instance?.PossessedEntity.GetComponent<Inventory>();
            if (inventory.items.Count > 0)
            {
                AuxiliaryBarHandler.Instance.SetText("Drop Item");
                var key = AuxiliaryBarHandler.Instance.gameObject.GetComponentInChildren<InteractKeyHandler>();
                if(key.KeyType != InteractKeyHandler.Key.DropItem)
                    key.KeyType = InteractKeyHandler.Key.DropItem;
            }
            else
                AuxiliaryBarHandler.Instance.HideBar();
            Debug.Log(inventory);
            InventoryHUD.Instance.ShowInventoryHUD(inventory);
        }
        if (PlayerInput.Instance.InPossession && !PossessionHandler.Instance.ChoosingPosition)
        {
            if (PossessionHandler.Instance.PossessedEntity.GetComponentInChildren<MultiTag>()?.HasTag("Mirror") == true)
            {
                PossessedController _controller = PossessionHandler.Instance.PossessedEntity.GetComponent<PossessedController>();
                var key = AuxiliaryBarHandler.Instance.gameObject.GetComponentInChildren<InteractKeyHandler>();
                if (_isRotating != _controller.RotationOnly || key.KeyType != InteractKeyHandler.Key.Interact)
                {
                    _isRotating = _controller.RotationOnly;
                    AuxiliaryBarHandler.Instance.SetText(_isRotating ? "Switch to move" : "Switch to rotate");
                    key.KeyType = InteractKeyHandler.Key.Interact;
                }
            }
            InteractionKeySpritehandler.Instance.setKey(InteractionKeySpritehandler.KeyType.QuitPossession);
            SmallNotificationBarHandler.Instance.SetText("Unpossess entity");
            SmallNotificationBarHandler.Instance.ShowBar();

            if ((PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard") && PossessionHandler.Instance.PossessedEntity.GetComponent<Inventory>().items.Count > 0) || PossessionHandler.Instance.PossessedEntity.GetComponentInChildren<MultiTag>()?.HasTag("Mirror") == true)
            {
                AuxiliaryBarHandler.Instance.ShowBar();
            }
        }
        else if (PossessionHandler.Instance.ChoosingPosition)
        {
            AuxiliaryBarHandler.Instance.HideBar();
            InteractionKeySpritehandler.Instance.setKey(InteractionKeySpritehandler.KeyType.Interact);
            SmallNotificationBarHandler.Instance.SetText("Choose position");
            SmallNotificationBarHandler.Instance.ShowBar();
        }
        if (!PlayerInput.Instance.InPossession && !ShadowHandler.Instance.CurrentPossessable)
        {
            SmallNotificationBarHandler.Instance.HideBar();
        }

    
    }
}
