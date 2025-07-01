using System.Collections;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
    void Start()
    {
        Camera.main.GetComponent<ThirdPersonCamera>().ForceSetCamera(Player.Instance.transform.position, -Player.Instance.transform.forward + Vector3.up * 0.1f);
        StartCoroutine(waitForFade());
    }

    IEnumerator waitForFade()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 10f;
    }
}
