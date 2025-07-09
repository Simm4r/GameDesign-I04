using System.Collections;
using UnityEngine;

public class FinalCuscene : MonoBehaviour
{
    [SerializeField] private DoorLargeOpener _doorOpener;
    [SerializeField] private ParticleSystem _sigil;
    [SerializeField] private FinalLaserBeam _laserL;
    [SerializeField] private FinalLaserBeam _laserR;
    [SerializeField] private ParticleSystem _instableSmoke;
    [SerializeField] private ParticleSystem _ruptureFlash;
    private GameObject _camera1;
    [SerializeField] private GameObject _camera2;
    
    private bool _triggered;

    void Awake()
    {
        _camera1 = Camera.main.gameObject;
    }
    void Update()
    {
        if (!_triggered && _laserL.StoneHit && _laserR.StoneHit && !Player.Instance.InCutscene && (!PlayerInput.Instance.InPossession || (PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity.CompareTag("Possessable_Guard"))) && Time.timeScale == 1.0f)
        {
            _triggered = true;
            Player.Instance.InCutscene = true;
            _instableSmoke.Play(true);
            StartCoroutine(StartCutscene());
        }
    }

    IEnumerator StartCutscene()
    {
        ScreenFadeController.Instance.FadeToBlack();
        var listener = MoveListener.Instance.GetComponent<AudioListener>();
        yield return new WaitForSecondsRealtime(1.5f);
        _camera1.SetActive(false);
        listener.enabled = false;
        _camera2.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSecondsRealtime(3.0f);
        _ruptureFlash.Play(true);
        _sigil.gameObject.SetActive(false);
        _instableSmoke.gameObject.SetActive(false);
        _laserL.gameObject.SetActive(false);
        _laserR.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(1.0f);
        _doorOpener.StartAnimation();
        yield return new WaitUntil(() => _doorOpener.GetState() == DoorLargeOpener.DoorState.Open);
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSecondsRealtime(1.5f);
        _camera1.SetActive(true);
        listener.enabled = true;
        _camera2.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSecondsRealtime(1.5f);
        Player.Instance.InCutscene = false;
    }
}
