using UnityEngine;

public class Item : Interactable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private InteractionHandler _caller;
    private GameObject _possessedEntity;
    private bool _canInteract = false;
    public ItemData ItemData
    {
        get { return _itemData; }
        set { _itemData = value; }
    }
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        Inventory inventory = _possessedEntity.GetComponent<Inventory>();

        if (inventory == null)
        {
            _possessedEntity = null;
            return;
        }

        bool inserted = inventory.AddItem(_itemData);
        if (inserted)
            Destroy(gameObject);
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
            _possessedEntity = PossessionHandler.Instance.PossessedEntity;

            _caller.Player = PossessionHandler.Instance.PossessedEntity;
            _canInteract = true;
        }

    }
}
