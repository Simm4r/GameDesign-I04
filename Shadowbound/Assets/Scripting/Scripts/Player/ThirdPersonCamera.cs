using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public float distance = 4f;
    public float height = 1.5f;
    public float rotationSpeed = 50f;
    public float pitchAngle = 15f;
    public bool invertX = false;
    public bool invertY = false;

    [Header("Mouse Sensitivity")]
    public float horizontalSensitivity = 3f;
    public float verticalSensitivity = 2f;
    [Header("Collision")]
    public LayerMask collisionLayers;     
    public float cameraRadius = 0.2f;
    public float minDistance = 0.5f;


    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y + 180f;
        pitch = pitchAngle;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {

        if (!player) return;

        float mouseX = PlayerInput.Instance.LookInput.x;
        float mouseY = PlayerInput.Instance.LookInput.y;


        yaw += (invertX ? -1 : 1) * mouseX * horizontalSensitivity;
        pitch -= (invertY ? -1 : 1) * mouseY * verticalSensitivity;
        pitch = Mathf.Clamp(pitch, 5f, 75f); // limit pitch

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetOffset = new Vector3(0, 0, -distance);
        Vector3 desiredCameraPos = player.position + (rotation * targetOffset) + Vector3.up * height;

        Vector3 rayOrigin = player.position + Vector3.up * height;
        Vector3 direction = (desiredCameraPos - rayOrigin).normalized;
        float targetDistance = distance;

        if (Physics.SphereCast(rayOrigin, cameraRadius, direction, out RaycastHit hit, distance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance - cameraRadius, minDistance, distance);
        }

        Vector3 correctedOffset = rotation * new Vector3(0, 0, -targetDistance);
        Vector3 finalPosition = player.position + correctedOffset + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * 10f);
        transform.LookAt(player.position + Vector3.up * 0.8f);

    }
}
