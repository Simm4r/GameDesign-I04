using UnityEngine;

public class HandleCamera : MonoBehaviour
{
    void Start()
    {
        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = ScreenFadeController.Instance.Animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(stateInfo.nameHash);
        if (!stateInfo.IsName("FadeOut") && !Player.Instance.InDialogue)
        {
            Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
            Invoke(nameof(WaitForCameraToBeStable),2.3f);
        }
    }

    void WaitForCameraToBeStable()
    {
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 10f;
        Destroy(this);
    }
}
