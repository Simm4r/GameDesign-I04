using System.Collections.Generic;
using UnityEngine;

public class ShadowDamageHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _shadowCastingLayers;
    [SerializeField] private float _heightOffset = 0.445f;
    [SerializeField] private float _lightThreshold = 0.5f;

    private Transform _playerTransform;
    private PlayerStats _playerStats;

    private bool _playerInShadow = true;
    private bool _canTakeDamage = true;
    private List<Light> _pointLights = new();

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
    }

    private bool IsInShadow()
    {
        Vector3 origin = _playerTransform.position + Vector3.down * _heightOffset;
        float totalLightIntensity = 0f;

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
                continue;
            }

            totalLightIntensity += pointLight.intensity / distanceToLight;

            if (totalLightIntensity >= _lightThreshold)
            {
                if (_playerInShadow)
                {
                    _playerInShadow = false;
                    _playerStats.ResetTimers();
                }
                return false;
            }
        }

        if (!_playerInShadow)
        {
            _playerInShadow = true;
            _playerStats.ResetTimers();
        }

        return true;
    }
}
