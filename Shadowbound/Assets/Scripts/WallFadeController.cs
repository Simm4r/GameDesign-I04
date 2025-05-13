using UnityEngine;

public class WallFadeController : MonoBehaviour
{
    private Material[] materials;
    private float targetThreshold = 0f;
    private float currentThreshold = 0f;
    private float fadeSpeed = 2f;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        materials = renderer.materials;

        foreach (var mat in materials)
        {
            mat.SetFloat("_AlphaClip", 1); // Enable alpha clip if needed
            mat.SetFloat("_AlphaClipThreshold", 0f); // Default visible
        }
    }

    void Update()
    {
        if (Mathf.Abs(currentThreshold - targetThreshold) > 0.01f)
        {
            currentThreshold = Mathf.Lerp(currentThreshold, targetThreshold, Time.deltaTime * fadeSpeed);
            foreach (var mat in materials)
            {
                mat.SetFloat("_AlphaClipThreshold", currentThreshold);
            }
        }
    }

    public void FadeOut() => targetThreshold = 0.95f;
    public void FadeIn() => targetThreshold = 0f;
}
