using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private ScreenFadeController _screenFade;
    [SerializeField] private float _respawnDelay = 1f;

    private void Awake()
    {
        _playerStats.OnPlayerDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        var controller = _playerStats.GetComponent<CharacterController>(); // Devo disattivare il Controller di Alex
        if (controller != null) controller.enabled = false;

        _screenFade.FadeToBlack();
        Invoke(nameof(RespawnPlayer), _respawnDelay);
    }

    private void RespawnPlayer()
    {               
        _playerStats.transform.position = _respawnPoint.position;

        var controller = _playerStats.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = true;


        _playerStats.ResetPlayer();
        _screenFade.FadeFromBlack();
    }
}
