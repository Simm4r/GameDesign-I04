using System.Collections;
using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    private Renderer _renderer;
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
    [SerializeField] private GameObject _camera3;
    [SerializeField] private LiftPillar _pillar;
    [SerializeField] private AudioSource _source;

    void Awake()
    {
        _camera1 = Camera.main.gameObject;
        _renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if (_hit && !_activeOnce)
        {
            _activeOnce = true;
            _source.Play();
            _mirror1.GetComponentInChildren<Possessable>().enabled = false;
            _mirror2.GetComponentInChildren<Possessable>().enabled = false;
            Player.Instance.InCutscene = true;
            StartCoroutine(Changecamera());
        }
        if (_activeOnce)
        {
            
        }
    }

    IEnumerator Changecamera()
    {
        ScreenFadeController.Instance.FadeToBlack();
        var originalListener = MoveListener.Instance.GetComponent<AudioListener>();
        yield return new WaitForSeconds(1.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        _camera1.SetActive(false);
        originalListener.enabled = false;
        _camera3.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        Material mat = _renderer.material;

        float elapsed = 0f;
        Color baseColor = Color.white;

        while (elapsed < 2.0f)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / 2.0f);
            
            // Emission intensity da 0 a 4, in gamma space
            float intensity = Mathf.Lerp(1f, 4f, t);

            mat.SetColor("_EmissionColor", baseColor * Mathf.Pow(intensity, 2f));

            yield return null;
        }
        mat.SetColor("_EmissionColor", baseColor * Mathf.Pow(4.0f, 2f));

        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(2.0f);
        _camera3.SetActive(false);
        _camera2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        _pillar.StartAnimation();
        yield return new WaitUntil(() => _pillar.State == LiftPillar.PillarState.Up);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        originalListener.enabled = true;
        _camera1.SetActive(true);
        _camera2.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSeconds(1.5f);
        Player.Instance.InCutscene = false;
        PossessionHandler.Instance.QuitImmediate = true;    
    }
}
