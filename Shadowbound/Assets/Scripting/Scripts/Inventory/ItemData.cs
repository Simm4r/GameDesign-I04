using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite icon;
    public GameObject worldPrefab;
    public string description;
}
