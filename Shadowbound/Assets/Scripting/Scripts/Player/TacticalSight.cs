using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TacticalSight : MonoBehaviour
{
    public static TacticalSight Instance { get; private set; }
    [SerializeField] private ParticleSystem _expandingSight;
    [SerializeField] private float _maxCooldown = 5f;

    private float _maxRadius;
    private float _effectDuration;
    private float _timer = 0.0f;
    private float _cooldown = 0.0f;
    private float _heightOffset = 0.15f;
    private HashSet<GameObject> _alreadyDetected = new();
    private List<GameObject> _rootObjects = new();

    public float ShadowVisionCooldown
    {
        get { return _cooldown; }
    }

    public float ShadowVisionMaxCooldown
    {
        get { return _maxCooldown; }
    }
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
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

                if (_alreadyDetected.Contains(hit.transform.root.gameObject))
                    continue;

                _alreadyDetected.Add(hit.transform.root.gameObject);
                _rootObjects.Add(hit.transform.root.gameObject);

                GameObject root = hit.transform.root.gameObject;
                OutlineHandler outlineHandler = root.GetComponentInChildren<OutlineHandler>();
                outlineHandler.UnoutlineEntity();
                outlineHandler.OutlineEntity(ref root);
            }
            else if (hit.gameObject.CompareTag("Interactable") && IsVisibleToCamera(hit.gameObject))
            {
                if (_alreadyDetected.Contains(hit.gameObject))
                    continue;

                _alreadyDetected.Add(hit.gameObject);
                _rootObjects.Add(hit.gameObject);

                GameObject root = hit.gameObject;
                OutlineHandler outlineHandler = root.GetComponentInChildren<OutlineHandler>();
                outlineHandler.UnoutlineEntity();
                outlineHandler.OutlineEntity(ref root);
            }
            else if (hit.transform.parent?.gameObject && hit.transform.parent.gameObject.CompareTag("Interactable") && IsVisibleToCamera(hit.transform.parent.gameObject))
            {
                if (_alreadyDetected.Contains(hit.transform.parent.gameObject))
                    continue;

                _alreadyDetected.Add(hit.transform.parent.gameObject);
                _rootObjects.Add(hit.transform.parent.gameObject);

                GameObject root = hit.transform.parent.gameObject;
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
            _alreadyDetected.Clear();
            HandleSightCollider();
        }
        else if (_expandingSight.IsAlive())
            HandleSightCollider();
    }

    void OnEnable()
    {
        _cooldown = 0.0f;
    }
    
    private bool IsVisibleToCamera(GameObject target)
    {
        Debug.Log(target);
        Camera cam = Camera.main;
        if (cam == null) return false;

        Renderer rend = target.GetComponentInChildren<Renderer>();
        if (rend == null) return false;

        
        Bounds bounds = rend.bounds;

        Vector3[] testPoints = new Vector3[5]
        {
            bounds.center,
            bounds.min,
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z)
        };

        foreach (Vector3 point in testPoints)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(point);

            bool inView = viewportPoint.z > 0 &&
                        viewportPoint.x > 0 && viewportPoint.x < 1 &&
                        viewportPoint.y > 0 && viewportPoint.y < 1;

            if (!inView)
                continue;
            Collider[] hits = Physics.OverlapSphere(Player.Instance.transform.position, 0.5f);
            if (hits.ToList().Contains(target.GetComponentInChildren<Collider>()))
                return true;
            Vector3 direction = (point - cam.transform.position).normalized;
            float distance = Vector3.Distance(cam.transform.position, point);

            if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit, distance))
            {
                if (hit.collider != target.GetComponentInChildren<Collider>())
                    continue;
            }
            Debug.Log("true");
            return true;
        }
        Debug.Log("false");
        return false;
    }
}
