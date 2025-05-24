using UnityEngine;
using UnityEngine.UI;

public class DissolveController : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed = 1f;

    private float dissolveAmount = 0f;
    private bool dissolving = false;

    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Outline targetOutline;
    [SerializeField] private ParticleSystem leftEye;
    [SerializeField] private ParticleSystem rightEye;
    [SerializeField] private Material fadeMaterial;

    private MaterialPropertyBlock propBlock;
    private UndissolveController _undissolveController;

    public bool IsDissolving
    {
        get { return dissolving; }
    }
    void Awake()
    {
        propBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_DissolveAmount", dissolveAmount);
        targetRenderer.SetPropertyBlock(propBlock);
        _undissolveController = GetComponent<UndissolveController>();
    }

    void Update()
    {
        if (dissolving)
        {
            dissolveAmount += Time.unscaledDeltaTime * dissolveSpeed;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);

            foreach (var renderer in targetRenderer.GetComponentsInChildren<Renderer>())
            {
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetFloat("_DissolveAmount", dissolveAmount);
                renderer.SetPropertyBlock(propBlock);
            }
            var leftEmission = leftEye.emission;
            leftEmission.rateOverTime = 0;

            var rightEmission = rightEye.emission;
            rightEmission.rateOverTime = 0;
        }
        if (dissolveAmount == 1 && dissolving)
        {
            Time.timeScale = 1.0f;
            dissolving = false;
            _undissolveController.enabled = true;
            enabled = false;
        }
    }

    public void StartDissolve()
    {
        targetRenderer.material = fadeMaterial;
        Time.timeScale = 0.05f;
        dissolveAmount = 0f;
        dissolving = true;
    }
}