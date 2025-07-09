using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ResetEntity : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private void Awake()
    {
        _initialPosition = transform.position;
        _initialRotation = transform.rotation;
    }

    void OnEnable()
    {
        StartCoroutine(Subscribe());
    }

    IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => PlayerStats.Instance != null);
        PlayerStats.Instance.OnPlayerDeath += ResetEvent;
    }

    void OnDisable()
    {
        PlayerStats.Instance.OnPlayerDeath -= ResetEvent;
    }

    private void ResetEvent()
    {
        Debug.Log("Resetta entità");
        StartCoroutine(ResetEntityCorutine(tag));
    }

    IEnumerator ResetEntityCorutine(string tag)
    {
        yield return new WaitForSeconds(2.0f);
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
            
        transform.SetPositionAndRotation(_initialPosition, _initialRotation);
        if (agent != null)
            agent.enabled = true;
        switch (tag)
            {
                case "Possessable_Guard":
                    GuardPatrol patrol = GetComponent<GuardPatrol>();
                    patrol?.ResetAgent();
                    Light torchLight = GetComponentInChildren<Light>();
                    GetComponent<GuardStats>().Torch.GetComponentInChildren<AudioSource>().Play();
                    ParticleSystem fire = torchLight.transform.parent.GetComponentInChildren<ParticleSystem>();
                    var emission = fire.emission;
                    torchLight.enabled = true;
                    emission.rateOverTime = 40.0f;
                    break;
                case "Possessable_Animal":
                    FleeingEntity entity = GetComponent<FleeingEntity>();
                    transform.Find("Capsule").gameObject.SetActive(true);
                    transform.Find("Rat").gameObject.SetActive(true);
                    entity?.ResetAgent();
                    break;
                default:
                    break;
            }
    }
}
