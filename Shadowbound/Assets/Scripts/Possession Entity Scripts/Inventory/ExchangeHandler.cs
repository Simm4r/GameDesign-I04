using UnityEngine;

public class ExchangeHandler : MonoBehaviour
{
    private bool _isEntityInRange = false;
    [SerializeField] private Inventory _entityInventory;
    private GameObject _targetEntity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
    }

    void Start()
    {
        _entityInventory = GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
        
    }
}
