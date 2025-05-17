using UnityEngine;

public class PossessableCueParticles : MonoBehaviour
{
    private ParticleSystem[] _particleSystems;

    private float _targetEmissionRate = 0f;
    private float _currentEmissionRate = 0f;

    [SerializeField] private float _activeRate = 40f;
    [SerializeField] private float _inactiveRate = 0f;

    [SerializeField] private float _lerpSpeed = 10f;

    void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        _currentEmissionRate = Mathf.Lerp(_currentEmissionRate, _targetEmissionRate, Time.deltaTime * _lerpSpeed);

        foreach (ParticleSystem ps in _particleSystems)
        {
            var emission = ps.emission;
            emission.rateOverTime = _currentEmissionRate;
        }
    }

    public void ShowPossessableCueParticles()
    {
        _targetEmissionRate = _activeRate;
    }

    public void HidePossessableCueParticles()
    {
        _targetEmissionRate = _inactiveRate;
    }
}
