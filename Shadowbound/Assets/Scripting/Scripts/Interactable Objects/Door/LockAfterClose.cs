using UnityEngine;

public class LockAfterClose : MonoBehaviour
{
    [SerializeField] private DoorOpener _doorOpener;
    [SerializeField] private DoorLocked _doorLocked;

    void Update()
    {
        if (!_doorOpener.IsOpen)
            _doorLocked.IsLocked = true;
    }
}
