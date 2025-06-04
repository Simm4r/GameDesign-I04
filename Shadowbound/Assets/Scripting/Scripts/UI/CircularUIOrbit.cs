using UnityEngine;

public class CircularUIOrbit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float orbitRadius = 0.2f;
    [SerializeField] private float heightOffset = 0.3f;
    [SerializeField] private float horizontalOffset = 0f;

    void Update()
    {
        if (target == null || Camera.main == null)
            return;

        Vector3 toPlayerFlat = Camera.main.transform.position - target.position;
        toPlayerFlat.y = 0;
        toPlayerFlat.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, toPlayerFlat);

        Vector3 orbitPosition = target.position
                              + Vector3.up * heightOffset
                              + toPlayerFlat * orbitRadius
                              + right * horizontalOffset;

        transform.position = orbitPosition;

        Vector3 lookDirection = Camera.main.transform.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.LookRotation(lookDirection);
        transform.Rotate(0, 180f, 0);
    }
}