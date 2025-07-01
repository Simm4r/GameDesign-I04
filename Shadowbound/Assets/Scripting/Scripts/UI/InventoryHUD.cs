using UnityEngine;
using UnityEngine.UI;

public class InventoryHUD : MonoBehaviour
{
    public static InventoryHUD Instance { get; set; }

    // [SerializeField] private Inventory _entityInventory;
    // [SerializeField] private List<Image> _slots;
    [SerializeField] private Image[] _slots;
    [SerializeField] private int _nSlots;

    [SerializeField] private int _header = 0;

    private CanvasGroup _canvasGroup;

    // public GameObject Entity
    // {
    //     set { _entity = value; }
    // }

    public void InitInventoryHUD()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        HideInventoryHUD();
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].enabled = true;
        }
        // _slots = new List<Image>();
    }

    public void ShowInventoryHUD(Inventory possessedInventory)
    {
        // _entityInventory = possessedInventory;
        Debug.Log("Show Inventory");
        if (possessedInventory == null)
        {
            Debug.Log("Entity null");
            return;
        }
        //     return;
        UpdateInventoryHUD(possessedInventory);
        _canvasGroup.alpha = 1f;
        // Create slots as many as possessedEntity has
        // RectTransform panelImageRect = GetComponent<RectTransform>();
        // _nSlots = inventory.Length;
        // for (int i = 0; i < _nSlots; i++)
        // {

        // }

    }

    public void HideInventoryHUD()
    {
        Debug.Log("Hide inventory");
        // ClearSlot();
        _canvasGroup.alpha = 0f;
    }

    public void UpdateInventoryHUD(Inventory possessedInventory)
    {
        ItemData item = possessedInventory.GetItemDataByPosition();
        if (item != null && _slots[_header] != null)
        {
            _slots[_header].sprite = item.icon;
            _slots[_header].enabled = true;
        }
            
    }

    public void ClearSlot()
    {
        if (_slots[_header].sprite != null)
        {
            _slots[_header].sprite = null;
            _slots[_header].enabled = false;
        }
            
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        InitInventoryHUD();
    }
}
