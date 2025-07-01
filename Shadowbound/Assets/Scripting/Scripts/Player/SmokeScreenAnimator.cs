using UnityEngine;

public class SmokeScreenAnimator : MonoBehaviour
{
    public static SmokeScreenAnimator Instance { get; private set; }
    private ParticleSystem _smoke;
    [SerializeField] private float _maxLifeTime = 5.0f;
    [SerializeField] private float _lifeTime = 0.0f;
    private SphereCollider _smokeCollider;
    private MeshRenderer _renderer;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _smoke = GetComponent<ParticleSystem>();
        _smokeCollider = GetComponentInChildren<SphereCollider>();
        _renderer = GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
        if (!_smoke.IsAlive())
            return;

        if (_lifeTime == _maxLifeTime)
        {
            StopAnimation();
            return;
        }

        _lifeTime += Time.deltaTime;
        _lifeTime = Mathf.Clamp(_lifeTime, 0.0f, _maxLifeTime);
    }
    public void StartAnimation()
    {
        var emission = _smoke.emission;
        emission.rateOverTime = 40.0f;
        _lifeTime = 0.0f;
        _smokeCollider.enabled = true;
        _renderer.enabled = true;
    }
    public void StopAnimation()
    {
        var emission = _smoke.emission;
        emission.rateOverTime = 0.0f;
        _lifeTime = 0.0f;
        _smokeCollider.enabled = false;
        _renderer.enabled = false;
    }
}
