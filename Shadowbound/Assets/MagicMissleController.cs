using UnityEngine;

public class MagicMissleController : MonoBehaviour
{
    [SerializeField] private SphereLauncher _sphere;
    [SerializeField] private bool _trylaunch = false;
    [SerializeField] private Transform _trialPos;
    void Update()
    {
        if (_trylaunch)
        {
            _trylaunch = false;
            if (_sphere.IsStopped)
                return;
            _sphere.transform.position = transform.position;
            _sphere.GetComponentInChildren<Light>().enabled = true;
            _sphere.gameObject.SetActive(true);
            _sphere.LaunchAuto(_trialPos.position, 20f, transform.root.gameObject);
        }
    }
}
