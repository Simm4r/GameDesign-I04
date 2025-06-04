using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    // [SerializeField] private Inventory _entityInventory;
    [SerializeField] private Image[] _slots;
    // [SerializeField] private int maxSlots = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int _header = 0;
    private int _maxSlots;

    public int Header
    {
        get { return _header; }
        set { _header = value; }
    }
    void Awake()
    {
        // _slots = new Image[3];
        _maxSlots = _slots.Length;

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].enabled = false;
        }
        Image panelImage = GetComponent<Image>();
        panelImage.enabled = false;

    }

    void Start()
    {
        // UpdateUI();
    }

    void Update()
    {
        // if (_maxSlots == 1 && _header == 0 && _slots[0].sprite == null)
        // {

        // }
    }

    // Update is called once per frame
    public void enableInventoryUI()
    {
        Image panelImage = GetComponentInChildren<Image>();
        panelImage.enabled = true;
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].enabled = true;
        }
    }
    public void UpdateSlot(Sprite sprite)
    {
        if (_header < _maxSlots)
        {
            _slots[_header].sprite = sprite;
            _slots[_header].enabled = true;
            _header++;
        }
        else
        {
            // You shouldn't arrive here (in theory)

            Debug.Log("Max Items already reached. Can't update");
        }

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
        if (_header >= _maxSlots)
        {
            _header--;
        }
        if (_header > 0)
            {
                _slots[_header].sprite = null;
                _header--;
            }
            else if (_header == 0)
            {
                _slots[_header].sprite = null;
                _slots[_header].enabled = false;
            }
            else
            {
                // You shouldn't arrive here (in theory)
                // Reset _header pointer
                _header = 0;
                Debug.Log("Inventory is empty. Can't remove item");
            }
    }
}
