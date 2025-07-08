using UnityEngine;

public class CatTriggerDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TableTrigger"))
        {
            Debug.Log("Cat entered table trigger!");
            ThirdPersonCamera.Instance.SetUnderTableMode(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TableTrigger"))
        {
            Debug.Log("Cat exited table trigger!");
            ThirdPersonCamera.Instance.SetUnderTableMode(false);
        }
    }
}
