using UnityEngine;

public class EntityInteractionHandler : MonoBehaviour
{

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private float _interactionRadius = 2f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private KeyCode _interactKey = KeyCode.E;
    [SerializeField] private KeyCode _dropKey = KeyCode.Q;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_interactKey) && _playerInput.InPossession)
        {
            CheckForInteraction();
        }
        else if (Input.GetKeyDown(_dropKey) && _playerInput.InPossession)
        {
            // Drop Item
            Inventory inventory = GetComponent<Inventory>();
            if (inventory == null)
            {
                Debug.Log("Not inventory");
                return;
            }
            ItemData itemToDrop = inventory.GetItemDataByPosition(); // item[0] by default
            if (itemToDrop == null)
            {
                Debug.Log("Not item found");
                return;
            }
            Vector3 dropPos = transform.position + transform.forward * 1.5f;
            inventory.DropItem(itemToDrop, dropPos);
        }
    }

    void CheckForInteraction()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _interactionRadius, _interactableLayer);
        foreach (var hit in hits)
        {
            // Debug.Log($"HIT: {hit.tag}");
            IInteractable interactable = hit.GetComponent<IInteractable>();
            Debug.Log($"HIT TAG: {hit.tag}");
            if (hit.CompareTag("Item"))
            {
                // Pick up Item
                Inventory inventory = GetComponent<Inventory>();
                if (inventory == null)
                {
                    Debug.Log("Not inventory");
                    return;
                }

                Debug.Log($"Try To pick Object");
                PickupItem item = hit.GetComponent<PickupItem>();
                // Debug.Log($"hit: {hit}");
                bool isAdded = inventory.AddItem(item.itemData);
                if (isAdded)
                {
                    Debug.Log($"Inserted {item.itemData.itemName}, {item.itemData.description}");
                    // Destroy(item);
                    Destroy(hit.gameObject);
                }
                return;
            }
            else if (hit.CompareTag("LockedDoor"))
            {
                Inventory inventory = GetComponent<Inventory>();
                if (inventory == null)
                {
                    Debug.Log("Not inventory");
                    return;
                }
                LockedDoorOpener lockedDoor = hit.GetComponent<LockedDoorOpener>();
                Debug.Log($"locked door {lockedDoor.Key}");
                ItemData item = inventory.Contains(lockedDoor.Key.id);
                if (item != null)
                {
                    lockedDoor.SetIfHasKey(item);
                }
            }
            if (interactable != null)
                {
                    interactable.Interact();
                    break;
                }
        }
    }

    void ODrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _interactionRadius);        
    }
}
