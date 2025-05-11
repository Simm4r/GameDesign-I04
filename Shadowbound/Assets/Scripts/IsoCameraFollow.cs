using UnityEngine;

public class IsoCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(-5f, 5f, -5f);
    public float smoothSpeed = 5f;
    public Vector3 fixedRotation = new Vector3(30f, 45f, 0f);

    void Start()
    {
        offset = new Vector3(-5f, 5f, -5f);
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(fixedRotation);
    }
}
