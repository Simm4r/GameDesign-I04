using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float distance = 4f;
    [SerializeField] private float height = 1.5f;
    [SerializeField] private float pitchAngle = 15f;
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;

    [Header("Mouse Sensitivity")]
    [SerializeField] private float horizontalSensitivity = 3f;
    [SerializeField] private float verticalSensitivity = 2f;

    [Header("Collision Settings")]
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float cameraRadius = 0.2f;
    [SerializeField] private float minDistance = 0.5f;

    [Header("Ceiling Detection")]
    [SerializeField] private float ceilingDistanceThreshold = 2.0f;
    [SerializeField] private float confinedHeight = 0.8f;
    [SerializeField] private float confinedDistance = 2.5f;
    [SerializeField] private float normalHeight = 1.5f;
    [SerializeField] private float normalDistance = 4f;

    private float yaw;
    private float pitch;
    private bool hasCeiling;
    public float distToCeiling;
    Vector3 finalCameraPos = Vector3.zero;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y + 180f;
        pitch = pitchAngle;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (!player) return;

 
        float mouseX = _input.LookInput.x;
        float mouseY = _input.LookInput.y;

        // Apply sensitivity and inversion
        yaw += (invertX ? -1 : 1) * mouseX * horizontalSensitivity;
        pitch -= (invertY ? -1 : 1) * mouseY * verticalSensitivity;
        pitch = Mathf.Clamp(pitch, 10f, 45f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 rayOrigin = player.position + Vector3.up * height;
        Vector3 cameraDir = rotation * Vector3.back;

        // Collision check behind player
        float targetDistance = distance;
        if (Physics.Raycast(rayOrigin, cameraDir, out RaycastHit hit, distance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance, minDistance, distance);
        }

        // Final camera position
        Vector3 verticalOffset = Vector3.up * height;
        Vector3 offsetBack = Vector3.zero;
        if (hasCeiling && distToCeiling < ceilingDistanceThreshold)
        {
            verticalOffset -= Vector3.up * 0.5f; // Move camera a bit down
            offsetBack = cameraDir.normalized * 0.5f; // Pull back the camera more in confined spaces
        }

        finalCameraPos = player.position + cameraDir * targetDistance + verticalOffset + offsetBack;
        transform.position = Vector3.Lerp(transform.position, finalCameraPos, Time.deltaTime * 10f);

        // Ceiling detection
        hasCeiling = Physics.Raycast(player.position + Vector3.up * 0.3f, Vector3.up, out RaycastHit upHit, height + 1.5f, collisionLayers);
        distToCeiling = Vector3.Distance(player.position, upHit.point);

        // Rotation based on space type
        if (hasCeiling)
        {
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else
        {
            transform.LookAt(player.position + Vector3.up * 0.8f);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
    }
}
