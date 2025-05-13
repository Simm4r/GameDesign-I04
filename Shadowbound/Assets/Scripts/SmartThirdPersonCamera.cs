using UnityEngine;

public class SmartThirdPersonCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 defaultOffset = new(0, 5, -8);
    public float followSpeed = 5f;
    public float cameraRadius = 0.3f;
    public LayerMask obstacleMask;

    private Vector3 currentOffset;

    void Start()
    {
        currentOffset = defaultOffset;
    }

    void LateUpdate()
    {
        if (!player) return;

        Vector3 desiredPosition = player.position + defaultOffset;
        Vector3 direction = desiredPosition - player.position;

        // Raycast or SphereCast to check if wall is in the way
        if (Physics.SphereCast(player.position, cameraRadius, direction.normalized, out RaycastHit hit, defaultOffset.magnitude, obstacleMask))
        {
            // Bring camera closer to just before hitting the wall
            float adjustedDistance = hit.distance - cameraRadius;
            adjustedDistance = Mathf.Clamp(adjustedDistance, 1f, defaultOffset.magnitude); // minimum distance
            currentOffset = direction.normalized * adjustedDistance;
        }
        else
        {
            // No wall, go back to default offset
            currentOffset = Vector3.Lerp(currentOffset, defaultOffset, Time.deltaTime * followSpeed);
        }

        // Final camera position
        Vector3 finalPos = player.position + currentOffset;
        transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * followSpeed);
        transform.LookAt(player.position + Vector3.up * 1.5f); // look at upper body
    }
}
