using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private InventoryUI _inventoryUI;
    public List<InventoryItem> items;
    [SerializeField] int maxItems = 3;

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
        Debug.Log($"Removed Item {item.data.itemName}");
        // _inventoryUI.UpdateUI();
        _inventoryUI.ClearSlot();

        // string inventoryStr = "";
        // foreach (InventoryItem i in items)
        // {
        //     inventoryStr = inventoryStr + " " + i.data.itemName;
        // }
        // Debug.Log($"inventario: {inventoryStr}");
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

    // public void ExchangeItem(ItemData itemToExchange, ItemData traderItem, Inventory traderInventory)
    // {
    //     RemoveItem(itemToExchange);
    //     traderInventory.RemoveItem(traderItem);

    //     AddItem(traderItem);
    //     traderInventory.AddItem(itemToExchange);
    // }

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
