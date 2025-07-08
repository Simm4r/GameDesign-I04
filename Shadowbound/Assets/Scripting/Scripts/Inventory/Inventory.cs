using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUI _inventoryUI;
    public List<InventoryItem> items;
    [SerializeField] int maxItems = 1;

    [SerializeField] private PossessedController _possessedController;
    public bool showInventory = false;

    public bool AddItem(ItemData newItem)
    {
        InventoryItem existingItem = items.Find(i => i.data == newItem);

        if (existingItem == null && items.Count < maxItems)
        {
            items.Add(new InventoryItem(newItem));
            Debug.Log($"Aggiunto {newItem.itemName}");
            _inventoryUI.UpdateSlot(newItem.icon);
            return true;

        }
        else if (existingItem == null && items.Count >= maxItems)
        {
            Debug.Log("Inventory full");
        }
        return false;
    }

    public void RemoveItem(ItemData itemToRemove)
    {
        InventoryItem item = items.Find(i => i.data == itemToRemove);

        if (item == null)
        {
            Debug.LogError("Item Not Found");
            return;
        }

        items.Remove(item);
        _inventoryUI.ClearSlot();
        InventoryHUD.Instance.ClearSlot();
        Debug.Log($"Removed Item {item.data.itemName}");
        // _inventoryUI.UpdateUI();

        // string inventoryStr = "";
        // foreach (InventoryItem i in items)
        // {
        //     inventoryStr = inventoryStr + " " + i.data.itemName;
        // }
        // Debug.Log($"inventario: {inventoryStr}");
    }

    public void DropItem(ItemData itemToDrop, Vector3 dropPosition)
    {
        InventoryItem item = items.Find(i => i.data == itemToDrop);
        if (item == null)
        {
            Debug.LogError("Item Not Found");
            return;
        }
        if (item.data.worldPrefab != null)
        {
            GameObject dropped = Instantiate(item.data.worldPrefab, dropPosition, Quaternion.identity);

            dropped.isStatic = false;
            InteractionHandler interactionHandler = dropped.GetComponent<InteractionHandler>();
            interactionHandler.Player = PlayerInput.Instance.InPossession ? PossessionHandler.Instance.PossessedEntity : null;
            Rigidbody rb = dropped.AddComponent<Rigidbody>();
            rb.useGravity = true;        // Per farlo cadere
            rb.mass = 5f;                // Un po’ pesante, ma dipende dall’oggetto
            rb.linearDamping = 5f;                // Frizione lineare per rallentare lo scivolamento
            rb.angularDamping = 5f;         // Frizione angolare per non farlo rotolare
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Per maggiore stabilità visiva
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Per collisioni accurate
            StartCoroutine(WaitUntilStableThenFreeze(dropped));
            /*
            Item pickedProperty = dropped.AddComponent<Item>();
            pickedProperty.ItemData = itemToDrop;
            */
        }
        RemoveItem(item.data);
    }

    private IEnumerator WaitUntilStableThenFreeze(GameObject dropped)
    {
        Rigidbody rb = dropped.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        yield return new WaitForSeconds(0.5f);

        while (rb != null && rb.linearVelocity.magnitude > 0.05f || rb.angularVelocity.magnitude > 0.05f)
            {
                if (rb == null) yield break;
                yield return null;
            }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Destroy(rb);
        }
    }
    public ItemData Contains(int idItem)
    {
        InventoryItem item = items.Find(i => i.data.id == idItem);

        if (item == null)
        {
            return null;
        }
        return item.data;
    }

    public ItemData GetItemDataByPosition(int position = 0)
    {
        if (position < 0 || position >= items.Count)
        {
            Debug.Log("Error, Invalid position");
            return null;
        }
        return items[position].data;
    }
    // Just for Debug
    public void PrintInventory()
    {
        string inventoryStr = "";
        foreach (InventoryItem i in items)
        {
            inventoryStr = inventoryStr + " " + i.data.itemName;
        }
        Debug.Log($"inventario: {inventoryStr}");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        // InventoryItem dummy = new InventoryItem(new ItemData());
        // dummy.data.itemName = "Dummy";
        // items.Add(dummy);
        items = new List<InventoryItem>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        showInventory = _possessedController.AlreadyPossessed;
        if (showInventory)
        {
            _inventoryUI.enableInventoryUI();
        }
    }
}
