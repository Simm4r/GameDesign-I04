using UnityEngine;

public class StepEnabler : MonoBehaviour
{
    [SerializeField] private DoorOpener _door;
    [SerializeField] private GameObject _entrance;
    [SerializeField] private GameObject _exit;

    void Update()
    {
        if (_door.IsOpen || (!_door.IsOpen && _door.IsAnimationStarted))
        {
            _entrance.SetActive(false);
            _exit.SetActive(false);
            return;
        }

        _entrance.SetActive(true);
        _exit.SetActive(true);
    }
}
