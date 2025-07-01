using System;
using UnityEngine;

public class EventTriggerer : MonoBehaviour
{
    [SerializeField] private RepeatableRead _read;
    private bool _reading;
    public event Action OnReadFinished;
    void Update()
    {
        if (_read.ReadingThis)
        {
            _reading = true;
        }

        if (_reading && _read.ReadingThis && ScrollHUDHandler.Instance.GetActualState() == "Idle")
        {
            OnReadFinished?.Invoke();
            enabled = false;
        }
            
    }
}
