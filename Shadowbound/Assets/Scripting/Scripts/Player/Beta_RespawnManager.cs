using KinematicCharacterController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Beta_RespawnManager : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private float _respawnDelay = 1f;
    [SerializeField] private KinematicCharacterMotor _motor;

    private void Awake()
    {
        _playerStats.OnPlayerDeath += HandleDeath;
        _motor = GetComponent<KinematicCharacterMotor>();
    }

    private void HandleDeath()
    {
        var controller = _playerStats.GetComponent<CharacterController>(); 
        if (controller != null) controller.enabled = false;

        ScreenFadeController.Instance.FadeToBlack();
        Invoke(nameof(RespawnPlayer), _respawnDelay);
    }

    private void RespawnPlayer()
    {
        _motor.SetPosition(Player.Instance.ActualCheckPoint.position);
        
        var controller = _playerStats.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = true;
        _playerStats.ResetPlayer();
        if (SceneManager.GetActiveScene().name == Player.Instance.ActualCheckPoint.scene)
            ScreenFadeController.Instance.FadeFromBlack();
        else
            SceneManager.LoadScene(Player.Instance.ActualCheckPoint.scene);
    }
}
