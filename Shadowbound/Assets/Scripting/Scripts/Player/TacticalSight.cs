using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TacticalSight : MonoBehaviour
{
    [SerializeField] private ParticleSystem _expandingSight;
    [SerializeField] private float _maxCooldown = 5f;

    private float _maxRadius;
    private float _effectDuration;
    private float _timer = 0.0f;
    private float _cooldown = 0.0f;
    private float _heightOffset = 0.15f;
    private HashSet<Collider> _alreadyDetected = new();
    private List<GameObject> _rootObjects = new();

    public float ShadowVisionCooldown {
        get { return _cooldown; }
    }

    public float ShadowVisionMaxCooldown
    {
        get { return _maxCooldown; }
    }
    void Awake()
    {
        _effectDuration = _expandingSight.main.startLifetime.constant;
        _maxRadius = _expandingSight.main.startSize.constant / 2;
    }
    private void HandleSightCollider()
    {
        _timer += Time.unscaledDeltaTime;
        
        float radius = Mathf.Lerp(0.0f, _maxRadius, _timer / _effectDuration);

        Collider[] hits = Physics.OverlapSphere(transform.position + Vector3.down * _heightOffset, radius);

        foreach (Collider hit in hits)
        {

            if (hit.transform.root.gameObject.tag.StartsWith("Possessable_"))
            {

                Debug.Log(hit.transform.root.gameObject.tag);
                if (_alreadyDetected.Contains(hit))
                    continue;

                _alreadyDetected.Add(hit);
                _rootObjects.Add(hit.transform.root.gameObject);

                GameObject root = hit.transform.root.gameObject;
                OutlineHandler outlineHandler = root.GetComponentInChildren<OutlineHandler>();
                outlineHandler.UnoutlineEntity();
                outlineHandler.OutlineEntity(ref root);
            }
        }

        if (_timer >= _effectDuration)
        {
            _cooldown = _maxCooldown;
            _timer = 0.0f;
        }
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

        if (PlayerInput.Instance.ShadowVision && !_expandingSight.IsAlive())
        {
            _expandingSight.Play();
            _timer = 0.0f;
            HandleSightCollider();
            _alreadyDetected.Clear();
        }
        else if (_expandingSight.IsAlive()) 
            HandleSightCollider();
    }

    void OnEnable()
    {
        _cooldown = 0.0f;
    }
}
