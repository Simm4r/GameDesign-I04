using UnityEngine;

public class EyeParticlesHandler : MonoBehaviour
{
    private ParticleSystem[] _eyes;
    private float _targetEmissionRate = 0f;
    private float _currentEmissionRate = 0f;
    [SerializeField] private float _activeRate = 40f;
    [SerializeField] private float _inactiveRate = 0f;

    [SerializeField] private float _lerpSpeed = 10f;

    void Awake()
    {
        _eyes = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        _currentEmissionRate = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, Time.deltaTime * _lerpSpeed);

        foreach (ParticleSystem eye in _eyes)
        {
            var emission = eye.emission;
            emission.rateOverTime = _currentEmissionRate;
        }
    }

    public void LitEyes()
    {
        _targetEmissionRate = _activeRate;
    }

    public void UnlitEyes()
    {
        _targetEmissionRate = _inactiveRate;
    }
}
