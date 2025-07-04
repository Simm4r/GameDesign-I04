using System.Collections;
using UnityEngine;

public class SetCamera : MonoBehaviour
{
    void Start()
    {
        ScreenFadeController.Instance.Animator.speed = 0;
        
        StartCoroutine(waitForFade());
    }

    IEnumerator waitForFade()
    {
        yield return new WaitUntil(() => Camera.main != null && Player.Instance != null);
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 1000f;
        Camera.main.GetComponent<ThirdPersonCamera>().ForceSetCamera(Player.Instance.transform.position, -Player.Instance.transform.forward);
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.Animator.speed = 1;
        yield return new WaitForSecondsRealtime(1.5f);
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 10f;
    }
}
