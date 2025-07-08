using System.Collections;
using KinematicCharacterController;
using Unity.VisualScripting;
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
    public float cameraRadius = 0.05f;
    public float minDistance = 0.5f;
    private Collider smokeCollider;
    private bool _inShadowStep = false;


    private float yaw = 0f;
    private float pitch = 0f;
    void Start()
    {

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y + 180f;
        pitch = pitchAngle;
        Cursor.lockState = CursorLockMode.Locked;
        smokeCollider = SmokeScreen.Instance.gameObject.GetComponentInChildren<SphereCollider>();
    }

    void LateUpdate()
    {
        if (!player || _inShadowStep) return;
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

        if (!_inShadowStep)
        {
            RaycastHit[] hits = Physics.SphereCastAll(rayOrigin, cameraRadius, direction, distance, collisionLayers);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                string rootTag = hit.collider.transform.root.tag;
                if (hit.collider == smokeCollider)
                    continue;
                if (rootTag.StartsWith("Possessable") && PlayerInput.Instance.InPossession && hit.collider.transform.root.gameObject == PossessionHandler.Instance.PossessedEntity)
                    continue;

                targetDistance = Mathf.Clamp(hit.distance - cameraRadius, minDistance, distance);
                break;

            }
        }
        Vector3 correctedOffset = rotation * new Vector3(0, 0, -targetDistance);
        Vector3 finalPosition = player.position + correctedOffset + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, finalPosition, Time.unscaledDeltaTime * rotationSpeed);
        transform.LookAt(player.position + Vector3.up * 0.8f);

    }

    public void ForceSetCamera(Vector3 focusPoint, Vector3 direction, float? customDistance = null)
    {
        Vector3 dir = direction.normalized;
        if (dir == Vector3.zero)
            dir = Vector3.forward;

        float dist = customDistance ?? distance; // Usa distanza personalizzata se fornita, altrimenti quella di default
        Debug.Log("CustomDistance" + dist);
        Vector3 cameraPos = focusPoint + (dir * dist) + Vector3.up * height;
        transform.position = cameraPos;

        // Guarda verso focusPoint
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

        // Aggiorna yaw e pitch (utile se usi rotazioni orbitanti dopo)
        Vector3 euler = transform.rotation.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
        if (Player.Instance.InShadowStep)
        {
            _inShadowStep = true;
            StartCoroutine(HandleStepFollow());
        }

    }

    IEnumerator HandleStepFollow()
    {
        Vector3 offset = transform.position - player.transform.position;
        Debug.Log("Sono Nella corutine : InShadowStep: " + _inShadowStep + " PlayerInShadowStep: " + Player.Instance.InShadowStep);
        float exitTimer = 0.0f;
        float maxTimer = 1.0f;

        while (_inShadowStep)
        {
            if (PlayerInput.Instance.Dying)
            {
                _inShadowStep = false;
                yield break;
            }
            transform.position = Vector3.Lerp(transform.position, player.transform.position + offset, Time.unscaledDeltaTime * rotationSpeed);
            if (!Player.Instance.InShadowStep && Player.Instance.GetComponent<KinematicCharacterMotor>().Velocity.magnitude != 0)
            {
                if (PlayerInput.Instance.MovementInput.z > 0)
                    exitTimer += PlayerInput.Instance.Sprint ? Time.deltaTime * 2 : Time.deltaTime;
                if (exitTimer >= maxTimer)
                    {
                        _inShadowStep = false;
                        yield break;
                    }
            }

            yield return null;
        }
    }
}
