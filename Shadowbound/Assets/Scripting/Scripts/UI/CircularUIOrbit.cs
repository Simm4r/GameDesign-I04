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

        // Calcolo il piano orizzontale dove orbitare (ignora l'altezza della camera)
        Vector3 cameraFlat = new Vector3(Camera.main.transform.position.x, target.position.y, Camera.main.transform.position.z);
        Vector3 toCameraFlat = (cameraFlat - target.position).normalized;

        // Calcolo la posizione orbitata
        Vector3 right = Vector3.Cross(Vector3.up, toCameraFlat);
        Vector3 orbitPosition = target.position
                              + Vector3.up * heightOffset
                              + toCameraFlat * orbitRadius
                              + right * horizontalOffset;

        transform.position = orbitPosition;

        // Rotazione: guarda direttamente la camera in 3D
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0f, 180f, 0f); // Solo se necessario
    }
}