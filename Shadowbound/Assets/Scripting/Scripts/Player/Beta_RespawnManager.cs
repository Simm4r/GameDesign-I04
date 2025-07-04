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
        {
            Destroy(this);
            return;
        }

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
        Camera.main.GetComponent<ThirdPersonCamera>().enabled = false;
        var emissionL = _eyeLeft.emission;
        var emissionR = _eyeRight.emission;
        emissionL.rateOverTime = 0.0f;
        emissionR.rateOverTime = 0.0f;
        Healthbar.Instance.gameObject.SetActive(false);
        StartCoroutine(FadeDelay());
    }

    private void RespawnPlayer()
    {
        _motor.SetPositionAndRotation(Player.Instance.ActualCheckPoint.position, Quaternion.LookRotation(Player.Instance.ActualCheckPoint.rotation * Vector3.forward));
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 1000f;
        Camera.main.GetComponent<ThirdPersonCamera>()?.ForceSetCamera(Player.Instance.transform.position, -Player.Instance.transform.forward);
        SmokeScreen.Instance.HasCharge = true;
        PlayerStats.Instance.HitByLaser = false;
        SmokeScreenAnimator.Instance.StopAnimation();
        Healthbar.Instance.gameObject.SetActive(true);
        PlayerStats.Instance.ResetPlayer();
        if (SceneManager.GetActiveScene().name == Player.Instance.ActualCheckPoint.scene)
            ScreenFadeController.Instance.FadeFromBlack();
        else
            SceneManager.LoadScene(Player.Instance.ActualCheckPoint.scene);
        PlayerInput.Instance.Dying = false;
        Camera.main.GetComponent<ThirdPersonCamera>().enabled = true;
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
        Camera.main.GetComponent<ThirdPersonCamera>().rotationSpeed = 10f;
        yield return new WaitForSeconds(1f);
        var emissionL = _eyeLeft.emission;
        var emissionR = _eyeRight.emission;
        emissionL.rateOverTime = 40f;
        emissionR.rateOverTime = 40f;
        PlayerStats.Instance.CanBeHit = true;
    }
}
