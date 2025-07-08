using System.Collections;
using UnityEngine;

public class EnlightGem : MonoBehaviour
{
    [SerializeField] private FinalLaserBeam _blue;
    private bool _hit = false;
    [SerializeField] private float _duration = 1.5f;
    private Renderer _renderer;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    void Update()
    {
        if (_blue.StoneHit && !_hit)
        {
            _hit = true;
            StartCoroutine(EnlightStone());
        }
    }

    IEnumerator EnlightStone()
    {
        Material mat = _renderer.material;

        float elapsed = 0f;
        Color baseColor = Color.white;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);
            
            // Emission intensity da 0 a 4, in gamma space
            float intensity = Mathf.Lerp(1f, 4f, t);

            mat.SetColor("_EmissionColor", baseColor * Mathf.Pow(intensity, 2f));

            yield return null;
        }


        mat.SetColor("_EmissionColor", baseColor * Mathf.Pow(4.0f, 2f));
    }
}
