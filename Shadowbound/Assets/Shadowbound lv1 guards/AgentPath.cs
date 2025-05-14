using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class AgentPath : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _empty1; // Primo empty object
    [SerializeField] private Transform _empty2; // Secondo empty object
    private Vector3 _destinationPosition;
    private float _timeAtPlaceTimer; // Tempo di attesa ora dinamico
    private float _timeSpent = 0f;
    private Animator _animator;
    public bool idling = false;
    [SerializeField] private float distanceFromDestination = 0f;

    // Attributi per il sistema di allerta
    [SerializeField] Transform target;
    [SerializeField] float alertRange = 4f;
    [SerializeField] float distanceToTarget = Mathf.Infinity;

    [SerializeField] Vector3 directionToTarget;
    void Start()
    { 
        _agent = gameObject.GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        if (_empty1 != null && _empty2 != null)
        {
            _destinationPosition = _empty1.position;
            _agent.SetDestination(_destinationPosition);
        }
        else
        {
            Debug.LogError("Gli Empty GameObject non sono assegnati!");
        }
        distanceFromDestination=Vector3.Distance(_agent.transform.position, _destinationPosition);
        SetRandomWaitTime(); // Imposta il primo valore casuale
    }

    void Update()
    {
        _animator.SetBool("Idling", idling);
        distanceFromDestination = Vector3.Distance(_agent.transform.position, _destinationPosition);
        

        Vector3 localOffset = new Vector3(0, 0, alertRange);
        Vector3 worldOffsetPos = transform.TransformPoint(localOffset);
        distanceToTarget = Vector3.Distance(worldOffsetPos, target.position);
        if (distanceToTarget <= alertRange)
        {
            Debug.Log("Area Target");
            Vector3 eyePosition = transform.position + Vector3.up * 0.5f;
            Vector3 directionToTarget = (target.position - eyePosition).normalized;
            if (Physics.Raycast(eyePosition, directionToTarget, out RaycastHit hit, alertRange))
            {
                if (hit.transform == target)
                {
                    Debug.Log("Vedo il target!");
                }
                else
                {
                    Debug.Log("Ostacolo: " + hit.transform.name);
                }
            }
        }


        if (idling && Vector3.Distance(_agent.transform.position, _destinationPosition) <= _agent.stoppingDistance) 
        {
            _timeSpent += Time.deltaTime;
            if (_timeSpent >= _timeAtPlaceTimer)
            {
                _timeSpent = 0f;

                
                // Cambia destinazione
                _destinationPosition = (_destinationPosition == _empty1.position) ? _empty2.position : _empty1.position;
                _agent.SetDestination(_destinationPosition);

                SetRandomWaitTime(); // Genera un nuovo tempo casuale
                idling = false; // Reset dello stato di attesa
               // Debug.Log($"Ho riposato quindi basta idle. remainingDistance: {Vector3.Distance(_agent.transform.position, _destinationPosition)}, stoppingDistance: {_agent.stoppingDistance}");

            }
        } else

        if (!idling && Vector3.Distance(_agent.transform.position, _destinationPosition) <= _agent.stoppingDistance) 
        {
            //Debug.Log($"Sono arrivato quindi vado in idle. remainingDistance: {Vector3.Distance(_agent.transform.position, _destinationPosition)}, stoppingDistance: {_agent.stoppingDistance}");
            idling = true;
        }
        

    }

    // Metodo per impostare un nuovo tempo di attesa casuale tra 10 e 15 secondi (valori interi)
    private void SetRandomWaitTime()
    {
        _timeAtPlaceTimer = Random.Range(5, 10); // Il secondo parametro è escluso, quindi 16 per ottenere fino a 15
    }

    private void OnDrawGizmosSelected() 
    {
        Vector3 localOffset = new Vector3(0, 0, alertRange);
        Vector3 worldOffsetPos = transform.TransformPoint(localOffset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldOffsetPos, alertRange);
    }
}