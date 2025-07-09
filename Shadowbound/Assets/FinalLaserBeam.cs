using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinalLaserBeam : MonoBehaviour
{
    [SerializeField] private GameObject _sigil;
    [SerializeField] private Collider _stoneCollider;
    [SerializeField] private AudioSource _source;
    private float _maxDistance = 100f;
    private LineRenderer _line;
    private bool _stoneHit = false;
    public bool StoneHit
    {
        get => _stoneHit;
    }

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = 0;
        _line.useWorldSpace = true;

        // Imposta larghezza della linea
        _line.startWidth = 0.05f;
        _line.endWidth = 0.05f;
    }

    void Update()
    {
        CastLaser();
    }

    private void CastLaser()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        points.Add(origin);

        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, _maxDistance)
            .OrderBy(h => h.distance) // Ordina per distanza
            .ToArray();

        if (hits.Length > 0)
        {
            var hit = hits[0];
            if (hit.collider == _stoneCollider)
            {
                if (!_source.isPlaying && !_stoneHit)
                    _source.Play();
                points.Add(hit.collider.gameObject.transform.position);
                points.Add(_sigil.transform.position);
                _line.positionCount = points.Count;
                _line.SetPositions(points.ToArray());
                _stoneHit = true;
                return;
            }
            _stoneHit = false;
            points.Add(hit.point);
            _line.positionCount = points.Count;
            _line.SetPositions(points.ToArray());
            return;
        }
        else
        {
            points.Add(origin + direction * _maxDistance);
            _line.positionCount = points.Count;
            _line.SetPositions(points.ToArray());
        }
    }
}
