using UnityEngine;

public class MagicMissleController : MonoBehaviour
{
    [SerializeField] private SphereLauncher _sphere;
    private bool _trylaunch = false;
    public bool TryLaunch
    {
        get => _trylaunch;
        set => _trylaunch = value;
    }
    private Vector3 _trialPos;
    public Vector3 TrialPos
    {
        get => _trialPos;
        set => _trialPos = value;
    }

    void Update()
    {
        if (_trylaunch && !_sphere.OnField)
        {
            _trylaunch = false;
            if (_sphere.IsStopped)
                return;
            _sphere.transform.position = transform.position;
            _sphere.GetComponentInChildren<Light>().enabled = true;
            _sphere.gameObject.SetActive(true);
            _sphere.LaunchAuto(_trialPos, 10f, transform.root.gameObject);
        }
        else
        {
            _trylaunch = false;
        }
    }
}
