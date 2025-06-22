using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter : MonoBehaviour
{
    [Header("Impostazioni del laser")]
    public float maxDistance = 100f;
    public int maxReflections = 3;

    private LineRenderer _line;

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

    void CastLaser()
    {
        List<Vector3> points = new List<Vector3>();

        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        points.Add(origin);

        for (int i = 0; i < maxReflections; i++)
        {
            if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance))
            {
                points.Add(hit.point);

                /*LaserReceiver receiver = hit.collider.GetComponent<LaserReceiver>();
                if (receiver != null)
                {
                    receiver.OnLaserHit();
                    break;
                }*/

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
                points.Add(origin + direction * maxDistance);
                break;
            }
        }

        _line.positionCount = points.Count;
        _line.SetPositions(points.ToArray());
    }
}