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

            Vector3 direction = dropPos - (transform.position + Vector3.up);

            if (Physics.Raycast(transform.position + Vector3.up, direction.normalized, out RaycastHit hit, direction.magnitude))
            {
                dropPos = hit.point - direction.normalized * 0.2f;
            }

            _inventory.DropItem(itemData, dropPos);
        }
    }
}
