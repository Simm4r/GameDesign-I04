using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShadowDamageHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _shadowCastingLayers;
    [SerializeField] private float _heightOffset = 0.445f;
    [SerializeField] private float _lightThreshold = 0.5f;
    [SerializeField] private float _inShadowObjectMaxTriggerDistance = 1.0f;

    private Transform _playerTransform;
    private PlayerStats _playerStats;
    private bool _foundGameObjectInPureShadow = false;

    private bool _playerInShadow = true;
    private bool _canTakeDamage = true;
    private List<Light> _pointLights = new();
    private List<GameObject> _possessableObjects = new();

    private Possessable _currentPossessable;

    private PossessionHandler _possessionHandler;
    public Possessable CurrentPossessable
    {
        get { return _currentPossessable; }
        set { _currentPossessable = value; }
    }

    private void Awake()
    {
        _playerTransform = transform;
        _playerStats = GetComponent<PlayerStats>();

        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in allLights)
        {
            if (light.type == LightType.Point && light.enabled)
            {
                _pointLights.Add(light);
            }
        }
        GameObject[] possessableGuards = GameObject.FindGameObjectsWithTag("Possessable_Guard");
        GameObject[] possessableObjects = GameObject.FindGameObjectsWithTag("Possessable_Object");
        GameObject[] possessableAnimals = GameObject.FindGameObjectsWithTag("Possessable_Animal");
        _possessableObjects.AddRange(possessableGuards);
        _possessableObjects.AddRange(possessableObjects);
        _possessableObjects.AddRange(possessableAnimals);
        _possessionHandler = GetComponent<PossessionHandler>();
    }

    private void Update()
    {
        if (!IsInShadow() && _canTakeDamage)
        {
            _playerStats.TakeDamage();
        }
        else
        {
            _playerStats.HealDamage();
        }
        CheckObjectsInPureShadow();
    }

    private bool IsInShadow()
    {
        Vector3 origin = _playerTransform.position + Vector3.down * _heightOffset;
        float totalLightIntensity = 0f;

        Possessable detectedPossessable = null;

        foreach (Light pointLight in _pointLights)
        {
            if (pointLight == null || !pointLight.enabled)
                continue;

            Vector3 directionToLight = pointLight.transform.position - origin;
            float distanceToLight = directionToLight.magnitude;

            if (distanceToLight > pointLight.range)
                continue;

            RaycastHit hit;
            if (Physics.Raycast(origin, directionToLight.normalized, out hit, distanceToLight, _shadowCastingLayers))
            {
                Possessable possessable = hit.collider.GetComponent<Possessable>();
                if (possessable != null)
                {
                    detectedPossessable = possessable;
                }
                // se il raycast colpisce uno shadow caster ignoro la fonte di luce
                continue;
            }

            // il raycast non ha colpito nulla, quindi considero la fonte di luce nel calcolo dell'intensità totale
            totalLightIntensity += pointLight.intensity / distanceToLight;

        }

        // aggiorno se necessario il _currentPossessable
        if (_currentPossessable != detectedPossessable && !_foundGameObjectInPureShadow)
        {
            if (_possessionHandler.PossessionCooldown == 0)
            {
                if (_currentPossessable != null)
                    _currentPossessable.HidePossessableCue();

                if (detectedPossessable != null)
                {
                    detectedPossessable.ShowPossessableCue();
                }

                _currentPossessable = detectedPossessable;
            }
            else
            {
                if (_currentPossessable != null)
                {
                    CurrentPossessable.HidePossessableCue();
                    _currentPossessable = null;
                }
            }
            
        }

        bool isInShadow = totalLightIntensity < _lightThreshold;

        if (_playerInShadow != isInShadow)
        {
            _playerInShadow = isInShadow;
            _playerStats.ResetTimers();
        }

        return isInShadow;

    }

    private void CheckObjectsInPureShadow()
    {
        GameObject foundGameObject = null;
        Collider momoCollider = GetComponent<Collider>();

        foreach (GameObject obj in _possessableObjects)
        {
            if ((obj.transform.position - transform.position).magnitude > _inShadowObjectMaxTriggerDistance)
                continue;

            Collider gameObjectCollider = obj.GetComponentsInChildren<Collider>().Where(c => c.enabled && c.gameObject.activeInHierarchy).ToArray()[0];
            Debug.Log("Object transform Position" + obj.transform.position);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, (obj.transform.position + 0.5f * Vector3.up - transform.position).normalized, (obj.transform.position + 0.5f * Vector3.up - transform.position).magnitude, _shadowCastingLayers);

            bool collided = false;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider == momoCollider || hit.collider == gameObjectCollider)
                    continue;
                collided = true;
                Debug.Log("Hitted a Collider " + hit.collider.name);
            }

            if (collided)
                continue;

            bool inLight = false;

            foreach (Light light in _pointLights)
            {
                if (light == null || !light.enabled)
                    continue;

                if ((light.transform.position - obj.transform.position).magnitude <= light.range)
                {
                    collided = false;
                    hits = Physics.RaycastAll(obj.transform.position, (light.transform.position - obj.transform.position).normalized, (light.transform.position - obj.transform.position).magnitude, _shadowCastingLayers);

                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider == momoCollider || hit.collider == gameObjectCollider)
                            continue;
                        collided = true;
                    }

                    if (!collided)
                    {
                        inLight = true;
                        foundGameObject = null;
                        break;
                    }
                }
            }

            if (!inLight)
            {
                foundGameObject = obj;
                break;
            }
        }
        if (_possessionHandler.PossessionCooldown == 0)
        {
            if (foundGameObject != null)
            {
                _foundGameObjectInPureShadow = true;
                if (_currentPossessable != null)
                {
                    _currentPossessable.HidePossessableCue();
                }
                _currentPossessable = foundGameObject.GetComponentInChildren<Possessable>();
                _currentPossessable.ShowPossessableCue();
            }
            else
                _foundGameObjectInPureShadow = false;
        }
        else
        {
            if (_currentPossessable != null)
                {
                    CurrentPossessable.HidePossessableCue();
                    _currentPossessable = null;
                }
        }

    }

private void OnDrawGizmosSelected()
{
    if (_possessableObjects == null)
        return;

    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, _inShadowObjectMaxTriggerDistance);

    Collider momoCollider = GetComponent<Collider>();

    foreach (GameObject obj in _possessableObjects)
    {
        if (obj == null)
            continue;

        float distance = (obj.transform.position - transform.position).magnitude;
        if (distance > _inShadowObjectMaxTriggerDistance)
        {
            Gizmos.color = Color.gray; // troppo lontano
            Gizmos.DrawLine(transform.position, obj.transform.position);
            continue;
        }

        Collider objectCollider = obj.GetComponentsInChildren<Collider>().FirstOrDefault(c => c.enabled && c.gameObject.activeInHierarchy);
        if (objectCollider == null)
            continue;

        RaycastHit[] shadowHits = Physics.RaycastAll(transform.position, (obj.transform.position - transform.position).normalized, distance, _shadowCastingLayers);
        bool hasObstacle = shadowHits.Any(hit => hit.collider != momoCollider && hit.collider != objectCollider);

        if (hasObstacle)
        {
            Gizmos.color = Color.red; // colpito da qualcosa
            Gizmos.DrawLine(transform.position, obj.transform.position);
            continue;
        }

        bool inLight = false;
        foreach (Light light in _pointLights)
        {
            if (light == null || !light.enabled)
                continue;

            float lightDistance = (light.transform.position - obj.transform.position).magnitude;
            if (lightDistance <= light.range)
            {
                RaycastHit[] lightHits = Physics.RaycastAll(obj.transform.position, (light.transform.position - obj.transform.position).normalized, lightDistance, _shadowCastingLayers);
                bool blockedByObstacle = lightHits.Any(hit => hit.collider != momoCollider && hit.collider != objectCollider);

                if (!blockedByObstacle)
                {
                    inLight = true;
                    break;
                }
            }
        }

        Gizmos.color = inLight ? Color.white : Color.green; // bianco = colpito dalla luce, verde = in ombra pura
        Gizmos.DrawLine(transform.position, obj.transform.position);
    }
}

}