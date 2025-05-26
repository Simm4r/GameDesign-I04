using Unity.VisualScripting;
using UnityEngine;

public class TacticalSight : MonoBehaviour
{
    [SerializeField] private ParticleSystem _expandingSight;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private float _maxCooldown = 5f;

    private float _cooldown = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

    }
    private void HandleSightCollider()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (_cooldown != 0)
        {
            _cooldown -= Time.unscaledDeltaTime;
            _cooldown = Mathf.Clamp(_cooldown, 0.0f, _maxCooldown);
            return;
        }

        if (_input.ShadowVision && !_expandingSight.IsAlive())
        {
            _expandingSight.Play();
            _cooldown = _maxCooldown;
        }
        else if (_expandingSight.IsAlive())
            HandleSightCollider();
    }

    void OnEnable()
    {
        _cooldown = 0.0f;
    }
}
