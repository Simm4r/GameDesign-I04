using System;
using UnityEngine;

public class PullLeverHandler : MonoBehaviour
{
    [SerializeField] PortcullisHandler portcull;
    [SerializeField] bool isOpenLever = false;
    private bool isEntityInRange = false;
    private GameObject entity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PossessableGrabberEntity"))
        {
            isEntityInRange = true;
            entity = other.gameObject;   
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PossessableGrabberEntity"))
        {
            isEntityInRange = false;
            entity = null;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isOpenLever = portcull.IsOpen;
        if (isEntityInRange && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Lever Pulled");
            portcull.IsOpen = !portcull.IsOpen;
        }
    }
}
