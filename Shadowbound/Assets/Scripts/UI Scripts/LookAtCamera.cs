using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _mainCam;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCam)
            transform.LookAt(transform.position + _mainCam.transform.forward);
    }
}
