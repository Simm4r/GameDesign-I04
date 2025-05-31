using KinematicCharacterController;
using UnityEngine;

public class Beta_RespawnManager : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private ScreenFadeController _screenFade;
    [SerializeField] private float _respawnDelay = 1f;
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private GameObject _lastSpawnpoint; // Ultimo punto di salvataggio

    private void Awake()
    {
        _playerStats.OnPlayerDeath += HandleDeath;
        _motor = GetComponent<KinematicCharacterMotor>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Il giocatore preme il tasto "Z"
        {
            FindSavableSpawnpoint();
        }
    }

    private void FindSavableSpawnpoint()
    {
        Saving[] savingObjects = FindObjectsOfType<Saving>(); // Trova tutti gli oggetti con lo script "Saving"

        foreach (Saving savingObj in savingObjects)
        {
            if (savingObj.getSavable()) // Controlla se è possibile salvare
            {
                _lastSpawnpoint = savingObj.gameObject; // Memorizza il GameObject come ultimo spawnpoint
                Debug.Log("Spawnpoint salvato: " + _lastSpawnpoint.name);
                return; // Esce dalla funzione dopo aver trovato il primo punto salvabile
            }
        }

        Debug.Log("Nessun punto di salvataggio disponibile.");
    }

    private void HandleDeath()
    {
        var controller = _playerStats.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        _screenFade.FadeToBlack();
        Invoke(nameof(RespawnPlayer), _respawnDelay);
    }

    private void RespawnPlayer()
    {
        if (_lastSpawnpoint != null) // Se è stato salvato un punto di respawn
        {
            _motor.SetPosition(_lastSpawnpoint.GetComponent<Saving>().GetPos());
            //Debug.Log("Respawn al punto salvato: " + _lastSpawnpoint.name);
        }
        else
        {
            _motor.SetPosition(_respawnPoint.position); // Respawn al punto predefinito
           // Debug.Log("Respawn al punto predefinito.");
        }

        var controller = _playerStats.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = true;

        _playerStats.ResetPlayer();
        _screenFade.FadeFromBlack();
    }
}
