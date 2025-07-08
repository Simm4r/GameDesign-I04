using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class AgentHandler : MonoBehaviour
{
    public static AgentHandler Instance { get; private set; }
    private List<NavMeshAgent> _agents = new();
    private List<MonoBehaviour> _scripts = new();
    private bool _inDialogue = false;
    private bool _inCutscene = false;
    private bool _inChase = false;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _background;
    [SerializeField] private AudioClip _chase;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _agents.Clear();
        _agents = FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None).ToList();
        _scripts = FindObjectsByType<GuardPatrol>(FindObjectsSortMode.None)
                    .Cast<MonoBehaviour>()
                    .Concat(FindObjectsByType<FleeingEntity>(FindObjectsSortMode.None))
                    .ToList();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (_inDialogue != Player.Instance.InDialogue || _inCutscene != Player.Instance.InCutscene)
        {
            _inDialogue = Player.Instance.InDialogue;
            _inCutscene = Player.Instance.InCutscene;

            if (_inDialogue || _inCutscene)
            {
                _scripts.ForEach(script => script.enabled = false);
                _agents.ForEach(agent => agent.enabled = false);
            }

            else
            {
                _agents.ForEach(agent => agent.enabled = true);
                _scripts.ForEach(script =>
                {
                    if (script is GuardPatrol gp)
                        gp.EnableAfterDialogue = true;
                    script.enabled = true;
                });
            }
        }

        IsAgentInChase();
        
    }

    private void IsAgentInChase()
    {
        var agents = _agents.Where(agent => agent.GetComponent<GuardPatrol>() != null && agent.enabled).Select(agent => agent.GetComponent<GuardPatrol>()).ToList();
        bool inChase = false;
        foreach (GuardPatrol agent in agents)
        {
            Debug.Log(agent);
            if (agent.CurrentState != GuardPatrol.GuardState.Chasing)
                continue;
            inChase = true;
            break;
        }

        if (_inChase != inChase)
        {
            _source.Stop();
            _inChase = inChase;
            if (inChase)
            {
                _source.resource = _chase;
            }
            else
                _source.resource = _background;

            _source.Play();
        }
    }
}
