using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<InventoryItem> items;
    [SerializeField] int maxItems = 3;

    [SerializeField] private PossessedController _possessedController;
    public bool showInventory = false;

    public void AddItem(ItemData newItem)
    {
        InventoryItem existingItem = items.Find(i => i.data == newItem);

        if (existingItem == null && items.Count < maxItems)
        {
            items.Add(new InventoryItem(newItem));
            Debug.Log($"Aggiunto {newItem.itemName}");

            // string inventoryStr = "";
            // foreach (InventoryItem i in items)
            // {
            //     inventoryStr = inventoryStr + " " + i.data.itemName;
            // }
            // Debug.Log($"inventario: {inventoryStr}");

        }
        else if (existingItem == null && items.Count >= maxItems)
        {
            Debug.Log("Inventory full");
        }
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
        Debug.Log($"Removed Item {item.data.itemName}");

        // string inventoryStr = "";
        // foreach (InventoryItem i in items)
        // {
        //     inventoryStr = inventoryStr + " " + i.data.itemName;
        // }
        // Debug.Log($"inventario: {inventoryStr}");
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
    }
}
