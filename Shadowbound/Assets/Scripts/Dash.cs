using UnityEngine;

public class Dash : MonoBehaviour
{
    public GameObject catModel;
    public GameObject dashModel;
    public bool enableDash = true;
    public float dashDistance = 5f;       // Forse un po' di meno
    public float dashEffectDuration = 0.2f; 
    public float cooldownDuration = 1.5f;

    private bool isDashing = false;
    private float effectTimer = 0f;
    private float cooldownTimer = 0f;
    private CharacterController characterController;
    private Vector3 dashTarget;
    private bool dashApplied = false;

    private void Awake()
    {
        if (catModel == null || dashModel == null)
        {
            Debug.LogWarning("catModel or dashModel not assigned!");
        }
        else
        {
            dashModel.SetActive(false);
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("Missing CharacterController!");
        }
    }

    private void Update()
    {
        if (!enableDash) return;

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (isDashing)
        {
            effectTimer -= Time.deltaTime;

            if (!dashApplied)
            {
                ApplyDash();
                dashApplied = true;
            }

            if (effectTimer <= 0f)
            {
                EndDash();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0f)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        Vector3 dashDirection = transform.forward;
        dashTarget = transform.position + dashDirection.normalized * dashDistance;

        catModel.SetActive(false);
        dashModel.SetActive(true);

        isDashing = true;
        dashApplied = false;
        effectTimer = dashEffectDuration;
        cooldownTimer = cooldownDuration;
    }

    private void ApplyDash()
    {
        characterController.enabled = false;
        transform.position = dashTarget;
        characterController.enabled = true;
    }

    private void EndDash()
    {
        catModel.SetActive(true);
        dashModel.SetActive(false);
        isDashing = false;
    }
}
