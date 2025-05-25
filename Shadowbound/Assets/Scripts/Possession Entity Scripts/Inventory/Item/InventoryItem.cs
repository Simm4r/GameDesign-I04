using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemData data;

    public InventoryItem(ItemData data)
    {
        this.data = data;
    }    
}
