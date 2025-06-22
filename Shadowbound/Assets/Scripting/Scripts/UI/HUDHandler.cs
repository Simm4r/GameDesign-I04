using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    [SerializeField] private GameObject _textBox;
    [SerializeField] private GameObject _cooldownBar;
    [SerializeField] private GameObject _scroll;
    [SerializeField] private GameObject _interactionBar;
    [SerializeField] private GameObject _NotificationBar;
    void Update()
    {
        if (Player.Instance.InDialogue)
        {
            _textBox.SetActive(true);
            _cooldownBar.SetActive(false);
            _interactionBar.SetActive(false);
            _NotificationBar.SetActive(false);
        }

        else
        {
            _textBox.SetActive(false);
            _cooldownBar.SetActive(true);
            _interactionBar.SetActive(true);
            _NotificationBar.SetActive(true);
        }

        if (Player.Instance.Reading)
        {
            _cooldownBar.SetActive(false);
            _interactionBar.SetActive(false);
            _NotificationBar.SetActive(false);
        }
        else
        {
            _cooldownBar.SetActive(true);
            _interactionBar.SetActive(true);
            _NotificationBar.SetActive(true);
        }

        if (PossessionHandler.Instance.ChoosingPosition)
        {
            _interactionBar.SetActive(false);
            SmallNotificationBarHandler.Instance.ShowBar();
            InventoryHUD.Instance.HideInventoryHUD();
        }
        else 
            _interactionBar.SetActive(true);

        if (!PlayerInput.Instance.InPossession)
        {
            SmallNotificationBarHandler.Instance.HideBar();
        }
        if (PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard") && !PossessionHandler.Instance.ChoosingPosition)
        {
            Inventory inventory = PossessionHandler.Instance?.PossessedEntity.GetComponent<Inventory>();
            InventoryHUD.Instance.ShowInventoryHUD(inventory);
        }
        else
        {
            InventoryHUD.Instance.HideInventoryHUD();
        }


    }
}
