using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private bool _isEnabled = false;
    // [SerializeField] private Inventory _entityInventory;
    [SerializeField] private Image[] _slots;
    // [SerializeField] private int maxSlots = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int _header = 0;
    private int _maxSlots;

    private CanvasGroup _canvasGroup;

    public int Header
    {
        get { return _header; }
        set { _header = value; }
    }

    public int MaxSlots
    {
        get { return _maxSlots; }
    }

    void Awake()
    {
        // _slots = new Image[3];
        initInventoryUI();
    }

    // Update is called once per frame
    public void enableInventoryUI()
    {
        Image panelImage = GetComponentInChildren<Image>();
        panelImage.enabled = true;
        for (int i = 0; i < _header; i++)
        {
            _slots[i].enabled = true;
        }
        _isEnabled = true;
    }

    public void initInventoryUI()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        HideInventoryUI();

        _maxSlots = _slots.Length;

        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].enabled = false;
        }
        Image panelImage = GetComponentInChildren<Image>();
        panelImage.enabled = false;
        _isEnabled = false;
    }

    public void ShowInventoryUI()
    {
        _canvasGroup.alpha = 1f;
    }

    public void HideInventoryUI()
    {
        _canvasGroup.alpha = 0f;
    }

    public void UpdateSlot(Sprite sprite)
    {
        if (_header < _maxSlots)
        {
            Debug.Log("Ho aggiunto");
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
            
            _slots[_header].enabled = false;
            _header--;
        }
        else if (_header == 0)
        {
            
            _slots[_header].sprite = null;
            Debug.Log(_slots[_header].name);
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

    public Image getSlot(int position = 0)
    {
        if (position < 0 || position > _header)
            return null;
        return _slots[position];
    }
}
