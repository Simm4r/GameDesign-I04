using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public ItemData itemData;
    private bool isEntityInRange = false;
    private GameObject entity;
    // [SerializeField] private PlayerInput _input;

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isEntityInRange && Input.GetKeyDown(KeyCode.E))
        {
            Inventory entityInventory = entity.GetComponent<Inventory>();

            if (entityInventory == null)
            {
                return;
            }
            Debug.Log($"Try To pick Object");
            bool isAdded = entityInventory.AddItem(itemData);
            if (isAdded)
            {
                Debug.Log($"Inserted {itemData.itemName}, {itemData.description}");
                Destroy(gameObject);
            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        // if(other.tag)
        if (!other.CompareTag("Untagged"))
        {
            Debug.Log($"Entry Key Item trigger by: {other.tag}");
        }

        if (other.CompareTag("Possessable_Guard"))
        {
            isEntityInRange = true;
            entity = other.gameObject;
            Debug.Log($"Entry Key Item trigger by: {other.tag} in possession");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Untagged"))
        {
            Debug.Log($"Exit Key Item trigger by: {other.tag}");
        }
        
        if (other.CompareTag("Possessable_Guard"))
        {
            isEntityInRange = false;
            entity = null;
            Debug.Log($"Exit Key Item trigger by: {other.tag}");
        }
    }
}
