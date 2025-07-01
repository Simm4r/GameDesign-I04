using System.Collections;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    private bool _hit = false;
    public bool Hit
    {
        get => _hit;
        set => _hit = value;
    }
    private bool _activeOnce = false;
    [SerializeField] private GameObject _mirror1;
    [SerializeField] private GameObject _mirror2;
    private GameObject _camera1;
    [SerializeField] private GameObject _camera2;
    [SerializeField] private LiftPillar _pillar;

    void Awake()
    {
        _camera1 = Camera.main.gameObject;
    }
    void Update()
    {
        if (_hit && !_activeOnce)
        {
            _activeOnce = true;
            _mirror1.GetComponentInChildren<Possessable>().enabled = false;
            _mirror2.GetComponentInChildren<Possessable>().enabled = false;
            ScreenFadeController.Instance.FadeToBlack();
            Player.Instance.InCutscene = true;
            StartCoroutine(Changecamera());
        }
        if (_activeOnce)
        {
            
        }
    }

    IEnumerator Changecamera()
    {
        yield return new WaitForSeconds(2.0f);
        _camera1.SetActive(false);
        _camera2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        _pillar.StartAnimation();
        yield return new WaitUntil(() => _pillar.State == LiftPillar.PillarState.Up);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        _camera1.SetActive(true);
        _camera2.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSeconds(1.5f);
        Player.Instance.InCutscene = false;
        PossessionHandler.Instance.QuitImmediate = true;    
    }
}
