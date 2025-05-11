using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float openAngle = 90f;        // Degrees to rotate when opening
    public float distance = 2f;          // Interaction distance
    public Transform pivotPoint;         // Rotation hinge point

    private bool isOpen = false;
    private Quaternion originalRotation;
    private Quaternion openRotation;

    void Start()
    {
        originalRotation = transform.rotation;
        openRotation = Quaternion.AngleAxis(openAngle, Vector3.up) * originalRotation;
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist <= distance && Input.GetButtonDown("Action"))
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (isOpen)
        {
            // Close the door
            transform.rotation = originalRotation;
        }
        else
        {
            // Open the door by rotating around the pivot
            transform.RotateAround(pivotPoint.position, Vector3.up, openAngle);
        }

        isOpen = !isOpen;
    }
}
