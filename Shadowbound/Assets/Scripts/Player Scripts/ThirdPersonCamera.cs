using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public float distance = 2f;
    public float height = 1.5f;
    public float smoothSpeed = 5f;
    public float rotationSmoothTime = 0.15f;
    public float pitchAngle = 15f;    

    private Vector3 currentVelocity = Vector3.zero;
    private float currentYaw;

    void LateUpdate()
    {
        if (!player) return;

        // Get the desired yaw from player rotation
        float targetYaw = player.eulerAngles.y;
        currentYaw = Mathf.SmoothDampAngle(currentYaw, targetYaw, ref currentVelocity.y, rotationSmoothTime);

        // Build rotation with yaw + pitch
        Quaternion rotation = Quaternion.Euler(pitchAngle, currentYaw, 0f);

        // Apply offset based on rotation
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = player.position + offset + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        Vector3 lookTarget = player.position + Vector3.up * 0.8f;
        transform.LookAt(lookTarget);
    }
}
