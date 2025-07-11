using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Impostazioni del laser")]
    public float maxDistance = 100f;
    public int maxReflections = 3;
    private List<CapsuleCollider> _collidersToIgnore = new();
    private LineRenderer _line;
    private List<Vector3> _points;
    private bool _stoneHit = false;
    void Awake()
    {
        List<MultiTag> objs = FindObjectsByType<MultiTag>(FindObjectsSortMode.None).ToList();
        objs = objs.Where(obj => obj.HasTag("Mirror")).ToList();
        var objList = objs.Select(obj => obj.transform.root);
        _collidersToIgnore = objList.Select(obj => obj.GetComponent<CapsuleCollider>()).ToList();
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
        if (_stoneHit)
        {
            HandlePlayerCollisionWithStableLaser();
            return;   
        }
        CastLaser();
    }

    private void HandlePlayerCollisionWithStableLaser()
    {
        if (_points == null || _points.Count < 2) return;

        for (int i = 0; i < _points.Count - 1; i++)
        {
            Vector3 start = _points[i];
            Vector3 end = _points[i + 1];
            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            Ray ray = new Ray(start, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, distance)
                .OrderBy(h => h.distance)
                .ToArray();



            foreach (var hit in hits)
            {
                // Ignora i mirror
                if (_collidersToIgnore.Contains(hit.collider))
                    continue;

                // Se colpisce il player
                if (!GodMode.Instance.godMode && hit.collider == Player.Instance.gameObject.GetComponent<CapsuleCollider>() && !PlayerInput.Instance.Dying && PlayerStats.Instance.CanBeHit)
                {
                    PlayerStats.Instance.CanBeHit = false;
                    PlayerStats.Instance.HitByLaser = true;
                    break;
                }
            }
        }
    }

    void CastLaser()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        points.Add(origin);


        for (int i = 0; i < maxReflections; i++)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance)
                .OrderBy(h => h.distance) // Ordina per distanza
                .ToArray();

            // Trova il primo collider valido (non ignorato)
            RaycastHit? validHit = hits.FirstOrDefault(h =>
                !_collidersToIgnore.Contains(h.collider) &&
                h.collider != Player.Instance.gameObject.GetComponent<CapsuleCollider>()
            );
            if (!GodMode.Instance.godMode && hits[0].collider == Player.Instance.gameObject.GetComponent<CapsuleCollider>() && !PlayerInput.Instance.Dying && PlayerStats.Instance.CanBeHit)
                {
                    PlayerStats.Instance.CanBeHit = false;
                    PlayerStats.Instance.HitByLaser = true;
                }
                
            if (validHit.HasValue)
            {
                RaycastHit hit = validHit.Value;
                LaserReceiver receiver = hit.collider.GetComponent<LaserReceiver>();
                if (receiver != null)
                {
                    points.Add(hit.collider.gameObject.transform.position);
                    _points = points;
                    _stoneHit = true;
                    receiver.Hit = true;
                    break;
                }

                points.Add(hit.point);
                if (hit.collider.CompareTag("Mirror"))
                {
                    direction = Vector3.Reflect(direction, hit.normal);
                    origin = hit.point + direction * 0.01f;
                }
                else
                {
                    break;
                }
            }
            else
            {
                // Nessun collider valido: vai dritto
                points.Add(origin + direction * maxDistance);
                break;
            }
        }

        _line.positionCount = points.Count;
        _line.SetPositions(points.ToArray());
    }
}

