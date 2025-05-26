using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // [SerializeField] private Inventory _entityInventory;
    [SerializeField] private Image[] _slots;
    // [SerializeField] private int maxSlots = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int _header = 0;
    void Awake()
    {
        _slots = new Image[3];
    }

    void Start()
    {
        // UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateSlot(Sprite sprite)
    {
        _slots[_header % 3].sprite = sprite;

        // for (int i = 0; i < _slots.Length; i++)
        // {
        //     if (i < _entityInventory.items.Count && _entityInventory.items[i] != null)
        //     {
        //         _slots[i].sprite = _entityInventory.items[i].data.icon;
        //         // _slots[i].enabled = true;
        //     }
        //     else
        //     {
        //         _slots[i].sprite = null;
        //         // _slots[i].enabled = false; // nasconde lo slot vuoto
        //     }
        // }
    }

    public void ClearSlot()
    {
        _slots[_header % 3] = null;
        _header--;
    }
}
