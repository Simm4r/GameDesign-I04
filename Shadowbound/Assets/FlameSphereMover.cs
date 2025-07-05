using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameSphereMover : MonoBehaviour
{
    [SerializeField] private List<Transform> _points;
    [SerializeField] private List<PathType> _types;
    [SerializeField] private float _speed = 1f;
    [SerializeField] private bool _startAnimation = false;
    private int _currentTargetIndex = 0;
    private float _t = 0f;
    private Vector3 _startPos;
    private bool _isMoving = false;

    public enum PathType
    {
        Curve,
        Straight
    }

    void Awake()
    {
        _startPos = transform.position;
        _t = 0f;
        _currentTargetIndex = 0;

        if (_points.Count == 0)
            Debug.LogWarning("No points assigned to FlameSphereMover");
        StartCoroutine(StartCutscene());
    }

    IEnumerator StartCutscene()
    {
        yield return new WaitUntil(() => Player.Instance != null);
        Player.Instance.InCutscene = true;
        yield return new WaitForSecondsRealtime(2.0f);
        StartMoving();
    }

    void Update()
    {
        if (_startAnimation)
        {
            _startAnimation = false;
            StartMoving();
        }
        if (!_isMoving) return;
        if (_points.Count == 0) return;

        Transform target = _points[_currentTargetIndex];
        PathType type = _types.Count > _currentTargetIndex ? _types[_currentTargetIndex] : PathType.Straight;

        _t += Time.deltaTime * _speed;

        if (type == PathType.Straight)
            MoveStraight(target);
        else
            MoveCurve(target);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            if (_currentTargetIndex == _points.Count - 1)
            {
                _isMoving = false;
                StartCoroutine(FinishCutscene());
            }
            else
            {
                _currentTargetIndex++;
                _t = 0f;
                _startPos = transform.position;
            }
        }
    }

    IEnumerator FinishCutscene()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Player.Instance.InCutscene = false;
        gameObject.SetActive(false);
    }

    private void MoveStraight(Transform target)
    {
        transform.position = Vector3.Lerp(_startPos, target.position, _t);
    }

private void MoveCurve(Transform target)
{
    Vector3 p0 = _startPos;
    Vector3 p2 = target.position;

    float y = p0.y;

    Vector2 p0_xz = new Vector2(p0.x, p0.z);
    Vector2 p2_xz = new Vector2(p2.x, p2.z);

    Vector2 midPoint = (p0_xz + p2_xz) / 2f;
    
    float curveOffset = 2f;

    Vector2 direction = (p2_xz - p0_xz).normalized;
    Vector2 perpendicular = new Vector2(-direction.y, direction.x);

    Vector2 p1_xz = midPoint + perpendicular * curveOffset;

    float t = Mathf.Clamp01(_t);

    Vector2 position_xz = (1 - t) * (1 - t) * p0_xz +
                          2 * (1 - t) * t * p1_xz +
                          t * t * p2_xz;

    Vector3 position = new Vector3(position_xz.x, y, position_xz.y);

    transform.position = position;
}

    public void StartMoving()
    {
        _isMoving = true;
        _startPos = transform.position;
        _t = 0f;
        _currentTargetIndex = 0;
        gameObject.SetActive(true);
    }

    public void StopMoving()
    {
        _isMoving = false;
    }
}
