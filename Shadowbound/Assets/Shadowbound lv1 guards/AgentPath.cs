using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Serialization;

public class AgentPath : MonoBehaviour
{
    private NavMeshAgent _agent;
    [SerializeField] private Transform _empty1; // Primo empty object
    [SerializeField] private Transform _empty2; // Secondo empty object
    [SerializeField] private Vector3 _destinationPosition;
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
    [SerializeField] private Transform alertMark;
    [SerializeField] private bool alerted = false;
    [SerializeField] private float alertedUrgency = 0f;
    [SerializeField] private int chaseStop = 10;
    [SerializeField] private bool chasing = false;
    private float _chaseTimer = 0f;
    private float _chaseDuration = 5f;
    private Vector3 _lastPos;
    void Start()
    {
        chasing = false;
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
        alertMark.localPosition = new Vector3(0, -20, 0);
        alerted = false;
    }

    void Update()
    {
        /*
        if (_timeAtPlaceTimer<0 && chaseStop>=10 
            && Vector3.Distance(_agent.transform.position, _destinationPosition) > _agent.stoppingDistance
            && Vector3.Distance(_destinationPosition, target.position)>1)
        {
            idling = false;
            _destinationPosition = (_destinationPosition == _empty1.position) ? _empty2.position : _empty1.position;
            _agent.SetDestination(_destinationPosition);
            SetRandomWaitTime();
        }*/

        _animator.SetBool("Idling", idling);
        distanceFromDestination = Vector3.Distance(_agent.transform.position, _destinationPosition);
        alertedUrgency= alertMark.transform.localScale.y;
        
        Vector3 localOffset = new Vector3(0, 0, alertRange);
        Vector3 worldOffsetPos = transform.TransformPoint(localOffset);
        distanceToTarget = Vector3.Distance(worldOffsetPos, target.position);
        if (distanceToTarget <= alertRange)
        {
            //Debug.Log("Area Target");
            Vector3 eyePosition = transform.position + Vector3.up * 0.5f;
            Vector3 directionToTarget = (target.position - eyePosition).normalized;
            if (Physics.Raycast(eyePosition, directionToTarget, out RaycastHit hit, alertRange))
            {
                if (hit.transform == target)
                {
                    Debug.Log("Vedo il target!");
                    alerted = true;
                }
                else
                {
                   // Debug.Log("Ostacolo: " + hit.transform.name);
                   if(alerted)startChasing();
                   alerted = false;
                }
            }
            else
            {
                if(alerted)startChasing();
                alerted = false;
            }
        }
        else
        {
            if(alerted)startChasing();
            alerted = false;
        }

        
        if (chasing && !alerted)
        {
            if (Vector3.Distance(_lastPos, transform.position) >=0.03)
            idling = false;
            _destinationPosition = target.position; _agent.SetDestination(_destinationPosition);
            _chaseTimer += Time.deltaTime;


        if (_chaseTimer >= _chaseDuration)
        {
            _destinationPosition = _empty2.position; _agent.SetDestination(_destinationPosition);
            chasing = false;
        }

        if (distanceToTarget > alertRange * 1.5)
        {
            _destinationPosition = _empty1.position; _agent.SetDestination(_destinationPosition);
            chasing = false;
        }
        }
        if (chaseStop > 10) chaseStop = 10;
        if (chaseStop < -1) chaseStop = -1;
        

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

        if (alerted)
        {
            alertMark.localPosition = new Vector3(0, 1.7f, 0);
            if (alertMark.transform.localScale.y < 0.25f) // Limite massimo
            {
                alertMark.transform.localScale += Vector3.one * 0.1f * Time.deltaTime;
            }
            else
            {
                if (chasing) chasing = false;
                _destinationPosition = transform.position; _agent.SetDestination(_destinationPosition);
                transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                idling = true;
            }
            /*else
            {
                if (_destinationPosition == target.position && chaseStop<0)
                {idling = true;
                    //_timeAtPlaceTimer = -1;
                }
                else
                {
                    if (_destinationPosition == target.position) chaseStop -= 1;
                    else
                    {
                        if (chaseStop < 10)
                        {
                            chaseStop += 1;
                        }
                        if (chaseStop >= 10)
                        {
                            idling = false;
                          //  _destinationPosition = _empty1.position; _agent.SetDestination(_destinationPosition);
                        }

                    }
                }

                _destinationPosition = target.position; _agent.SetDestination(_destinationPosition);
            }*/
        }
        else
        {
            if (!chasing){
            alertMark.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            alertMark.localPosition = new Vector3(0, -20, 0);
            chaseStop += 1;
        }}

        if (Vector3.Distance(_lastPos, transform.position) >= 0.03)
            idling = false;
        else idling = true;
        _lastPos = transform.position;
    }

    // Metodo per impostare un nuovo tempo di attesa casuale tra 10 e 15 secondi (valori interi)
    private void SetRandomWaitTime()
    {
        _timeAtPlaceTimer = Random.Range(5, 10); // Il secondo parametro è escluso, quindi 16 per ottenere fino a 15
    }

    private void startChasing()
    {
        chasing = true;
        _chaseDuration = 5f;
        _chaseTimer = 0f;
    }

    private void OnDrawGizmosSelected() 
    {
        Vector3 localOffset = new Vector3(0, 0, alertRange);
        Vector3 worldOffsetPos = transform.TransformPoint(localOffset);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(worldOffsetPos, alertRange);
    }
}