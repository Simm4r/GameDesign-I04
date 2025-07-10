using System;
using System.Collections;
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
    private bool _inCorutine = false;
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
        if(PlayerInput.Instance.Dying) {
            _source.Stop();
            _source.resource = _background;
            _source.volume = 1.0f;
            _source.Play();
        }
        var agents = _agents.Where(agent => agent.GetComponent<GuardPatrol>() != null && agent.enabled).Select(agent => agent.GetComponent<GuardPatrol>()).ToList();
        bool inChase = false;
        foreach (GuardPatrol agent in agents)
        {
            if (agent.CurrentState != GuardPatrol.GuardState.Chasing)
                continue;
            inChase = true;
            break;
        }

        if (_inChase != inChase)
        {
            _inChase = inChase;
            if (_inCorutine)
                return;
            if (inChase)
                {
                    _source.Stop();
                    _source.resource = _chase;
                    _source.volume = 1.0f;
                    _source.Play();
                }
                else
                {
                    if (_source.isPlaying)
                    {
                        StartCoroutine(CrossFadeChase());
                    }
                    else
                    {
                        _source.Stop();
                        _source.resource = _background;
                        _source.volume = 1.0f;
                        _source.Play();
                    }

                }


        }
    }

    IEnumerator CrossFadeChase()
    {
        _inCorutine = true;
        while (_source.volume != 0)
        {
            if (PlayerInput.Instance.Dying)
            {
                _inCorutine = false;
                _source.Stop();
                _source.resource = _background;
                _source.volume = 1.0f;
                _source.Play();
                yield break;
            }
                
            if (!_inChase)
                {
                    _source.volume = Mathf.Clamp01(_source.volume - Time.deltaTime / 8);
                    Debug.Log(_source.volume);
                }
                else
                    _source.volume = Mathf.Clamp01(_source.volume + Time.deltaTime / 2);

            yield return null;
        }
        _source.Stop();
        _source.volume = 1.0f;
        _source.resource = _background;
        _source.Play();
        _inCorutine = false;
    }
}
