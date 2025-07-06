using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SphereLauncher : MonoBehaviour
{
    [SerializeField] private float _maxLifetime = 5.0f;
    List<Collider> colliders = new();
    private float _lifeTime = 0.0f;   
    private Rigidbody rb;
    private bool isStopped = false;
    public bool IsStopped
    {
        get => isStopped;
    }

    public bool OnField = false;
    private GameObject _caller;
    void OnEnable()
    {
        List<GameObject> agents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList().Select(obj => obj.transform.root.gameObject).ToList();
        agents.ForEach(agent =>
        {
            List<Collider> colls = agent.GetComponentsInChildren<Collider>().ToList();
            colliders.AddRange(colls);
        });
        colliders.ForEach(collider =>
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collider);
        });
            Physics.IgnoreCollision(Player.Instance.gameObject.GetComponent<Collider>(), GetComponent<Collider>());
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        GetComponentInChildren<Light>().enabled = false;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (isStopped)
        {
            _lifeTime = Mathf.Clamp(_lifeTime + Time.deltaTime, 0.0f, _maxLifetime);
            if (_lifeTime == _maxLifetime)
            {
                isStopped = false;
                GetComponentInChildren<Light>().enabled = false;
                gameObject.SetActive(false);
                OnField = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isStopped) return;

        // Ferma il movimento
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        isStopped = true;
        _lifeTime = 0.0f;
    }
    public void LaunchAuto(Vector3 target, float launchSpeed, GameObject caller, bool useHighArc = false)
    {
        OnField = true;
        rb.constraints = RigidbodyConstraints.None;

        _caller = caller;

        Vector3 origin = transform.position;
        Vector3 delta = target - origin;

        Vector3 deltaXZ = new Vector3(delta.x, 0, delta.z);
        float distanceXZ = deltaXZ.magnitude;
        float heightDifference = delta.y;
        float gravity = Mathf.Abs(Physics.gravity.y);

        float speedSquared = launchSpeed * launchSpeed;

        float discriminant = speedSquared * speedSquared - gravity * (gravity * distanceXZ * distanceXZ + 2 * heightDifference * speedSquared);

        Vector3 velocity;

        if (discriminant < 0f)
        {

            Vector3 direction = deltaXZ.normalized;

            float angle = Mathf.Deg2Rad * 30f;

            velocity = direction * launchSpeed * Mathf.Cos(angle);
            velocity.y = launchSpeed * Mathf.Sin(angle);
        }
        else
        {
            float root = Mathf.Sqrt(discriminant);
            float lowAngle = Mathf.Atan((speedSquared - root) / (gravity * distanceXZ));
            float highAngle = Mathf.Atan((speedSquared + root) / (gravity * distanceXZ));
            float angle = useHighArc ? highAngle : lowAngle;

            Vector3 direction = deltaXZ.normalized;
            velocity = direction * launchSpeed * Mathf.Cos(angle);
            velocity.y = launchSpeed * Mathf.Sin(angle);
        }

        rb.linearVelocity = velocity;
    }
}
