using UnityEngine;

public class Dash : MonoBehaviour
{
    public GameObject catModel; // Il modello originale
    public GameObject dashModel; // Il modello alternativo
    public bool enableDash = true; // Master switch: Se false, il dash non funziona
    public float dashDistance = 0.1f; // Distanza totale del dash
    public float dashDuration = 0.5f; // Durata del dash

    private bool canSwitch = true;
    private float cooldownTimer = 0f;
    private float switchTimer = 0f;
    private bool isSwitched = false;
    private CharacterController characterController;
    private Vector3 dashVelocity;

    private void Awake()
    {
        if (catModel == null || dashModel == null)
        {
            Debug.LogWarning("Attenzione: catModel o dashModel non sono stati assegnati!");
        }
        else
        {
            dashModel.SetActive(false); // Disattiva dashModel all'inizio
        }

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("Errore: Il GameObject non ha un CharacterController!");
        }
    }

    private void Update()
    {
        if (!enableDash) return; // Se il master switch è OFF, esce subito dal metodo Update

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isSwitched)
        {
            switchTimer -= Time.deltaTime;

            // Movimento con CharacterController
            if (characterController != null)
            {
                characterController.Move(dashVelocity * Time.deltaTime);
            }

            if (switchTimer <= 0)
            {
                catModel.SetActive(true);
                dashModel.SetActive(false);
                isSwitched = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && canSwitch && cooldownTimer <= 0)
        {
            catModel.SetActive(false);
            dashModel.SetActive(true);
            isSwitched = true;
            switchTimer = dashDuration; // Tempo di visibilità di dashModel
            cooldownTimer = 3f; // Avvia subito il cooldown alla pressione del tasto

            // Configura la velocità di movimento per il dash
            dashVelocity = transform.forward * (dashDistance / dashDuration);
        }
    }
}
