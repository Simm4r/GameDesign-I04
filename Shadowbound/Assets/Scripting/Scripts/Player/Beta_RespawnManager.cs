using System.Collections;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Beta_RespawnManager : MonoBehaviour
{
    public static Beta_RespawnManager Instance { get; private set; }
    [SerializeField] private float _respawnDelay = 1f;
    [SerializeField] private KinematicCharacterMotor _motor;
    [SerializeField] private ParticleSystem _eyeLeft;
    [SerializeField] private ParticleSystem _eyeRight;

    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;

        _motor = GetComponent<KinematicCharacterMotor>();
    }

    private void Start()
    {
        PlayerStats.Instance.OnPlayerDeath += HandleDeath;
    }

    private void HandleDeath()
    {
        PlayerInput.Instance.Dying = true;

        var emissionL = _eyeLeft.emission;
        var emissionR = _eyeRight.emission;
        emissionL.rateOverTime = 0.0f;
        emissionR.rateOverTime = 0.0f;
        Healthbar.Instance.gameObject.SetActive(false);
        StartCoroutine(FadeDelay());
    }

    private void RespawnPlayer()
    {
        _motor.SetPosition(Player.Instance.ActualCheckPoint.position);

        PlayerInput.Instance.Dying = false;
        _motor.SetPosition(transform.position);
        Healthbar.Instance.gameObject.SetActive(true);
        PlayerStats.Instance.ResetPlayer();
        if (SceneManager.GetActiveScene().name == Player.Instance.ActualCheckPoint.scene)
            ScreenFadeController.Instance.FadeFromBlack();
        else
            SceneManager.LoadScene(Player.Instance.ActualCheckPoint.scene);
        StartCoroutine(EyesLitDelay());
    }

    IEnumerator FadeDelay()
    {
        yield return new WaitForSeconds(1f);
        ScreenFadeController.Instance.FadeToBlack();
        Invoke(nameof(RespawnPlayer), _respawnDelay);
    }

    IEnumerator EyesLitDelay()
    {
        yield return new WaitForSeconds(1f);
        var emissionL = _eyeLeft.emission;
        var emissionR = _eyeRight.emission;
        emissionL.rateOverTime = 40f;
        emissionR.rateOverTime = 40f;
    }
}
