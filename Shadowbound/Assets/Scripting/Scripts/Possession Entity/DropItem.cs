using UnityEngine;

public class DropItem : MonoBehaviour
{
    private Inventory _inventory;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }
    void Update()
    {
        if (_inventory == null)
            return;

        if (PlayerInput.Instance.DropItem)
        {
            ItemData itemData = _inventory.GetItemDataByPosition();

            if (itemData == null)
                return;

            Vector3 dropPos = transform.position + transform.forward * 1f + Vector3.up;
            _inventory.DropItem(itemData, dropPos);
        }
    }
}
