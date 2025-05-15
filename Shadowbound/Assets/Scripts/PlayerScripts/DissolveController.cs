using UnityEngine;

public class DissolveController : MonoBehaviour
{
    [SerializeField] private float dissolveSpeed = 0.1f;
    [SerializeField] private string dissolveProperty = "_DissolveAmount";

    private float dissolveAmount = 0f;
    private bool dissolving = false;

    [SerializeField] private Renderer targetRenderer;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        propBlock = new MaterialPropertyBlock();
        targetRenderer.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_DissolveAmount", dissolveAmount);
        targetRenderer.SetPropertyBlock(propBlock);
    }

    void Update()
    {
        if (dissolving)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveAmount = Mathf.Clamp01(dissolveAmount);

            targetRenderer.GetPropertyBlock(propBlock);
            propBlock.SetFloat("_DissolveAmount", dissolveAmount);
            targetRenderer.SetPropertyBlock(propBlock);
        }
    }

    public void StartDissolve()
    {
        dissolveAmount = 0f;
        dissolving = true;
    }
}