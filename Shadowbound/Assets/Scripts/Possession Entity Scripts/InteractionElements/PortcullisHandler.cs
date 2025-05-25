using UnityEngine;

public class PortcullisHandler : MonoBehaviour
{
    [SerializeField] private bool _isOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool IsOpen
    {
        get { return _isOpen; }
        set { _isOpen = value; }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
