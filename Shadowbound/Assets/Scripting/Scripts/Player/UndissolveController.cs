using UnityEngine;
using UnityEngine.UI;

public class UndissolveController : MonoBehaviour
{
    public static UndissolveController Instance { get; private set; }
    [SerializeField] private float _undissolveSpeed = 1f;

    private float _undissolveAmount = 1f;
    private bool _undissolving = false;

    [SerializeField] private Renderer _targetRenderer;
    [SerializeField] private Outline _targetOutline;
    [SerializeField] private ParticleSystem _leftEye;
    [SerializeField] private ParticleSystem _rightEye;
    [SerializeField] private Material _originalMaterial;
    [SerializeField] private float objectHeight = 0.305f;
    [SerializeField] private Outline _outline;

    private MaterialPropertyBlock _propBlock;

    public bool IsUndissolving
    {
        get { return _undissolving; }
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        _propBlock = new MaterialPropertyBlock();
        _targetRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_DissolveAmount", _undissolveAmount);
        _propBlock.SetFloat("_ObjectHeight", objectHeight);
        enabled = false;
    }

    void Update()
    {
        if (PauseHandler.Instance.InPause)
            return;
        if (_undissolving)
        {
            _undissolveAmount -= Time.unscaledDeltaTime * _undissolveSpeed;
            _undissolveAmount = Mathf.Clamp01(_undissolveAmount);

            foreach (var renderer in _targetRenderer.GetComponentsInChildren<Renderer>())
            {
                renderer.GetPropertyBlock(_propBlock);
                _propBlock.SetFloat("_DissolveAmount", _undissolveAmount);
                renderer.SetPropertyBlock(_propBlock);
            }
            var leftEmission = _leftEye.emission;
            leftEmission.rateOverTime = 40;

            var rightEmission = _rightEye.emission;
            rightEmission.rateOverTime = 40;
        }
        if (_undissolveAmount == 0 && _undissolving)
        {
            _targetRenderer.material = _originalMaterial;
            Time.timeScale = 1.0f;
            _undissolving = false;
            PossessionHandler.Instance.ChoosingPosition = false;
            DissolveController.Instance.enabled = true;
            enabled = false;
        }
    }

    public void StartUndissolve()
    {
        _outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        Time.timeScale = 0.05f;
        _undissolveAmount = 1f;
        _undissolving = true;
    }
}