using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    [SerializeField] private GameObject _torch;
    [SerializeField] private ParticleSystem _sleepAura;
    [SerializeField] private ParticleSystem _scareAura;
    [SerializeField] private ParticleSystem _speedAura;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip _fearSound;
    [SerializeField] private AudioClip _speedSound;
    [SerializeField] private AudioClip _sleepSound;
    public event Action<GuardStatus> OnStatusChanged;
    public enum GuardStatus
    {
        Sleepy,
        Scared,
        Fastened,
        None
    }
    [SerializeField] private List<float> _statusMaxTime;

    private GuardStatus _status = GuardStatus.None;
    private float _progress = 0.0f;
    public GuardStatus Status
    {
        get => _status;
        set
        {
            _progress = 0.0f;
            switch (value)
            {
                case GuardStatus.Sleepy:
                    _source.Stop();
                    _source.resource = _sleepSound;
                    _source.Play();
                    _sleepAura.Play(true);
                    break;
                case GuardStatus.Scared:
                    StartCoroutine(WaitForScare());
                    break;
                case GuardStatus.Fastened:
                    _source.Stop();
                    _source.resource = _speedSound;
                    _source.Play();
                    _speedAura.Play(true);
                    break;
                case GuardStatus.None:
                    if (PlayerInput.Instance.Dying)
                    {
                        _source.Stop();
                    }
                    _sleepAura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _scareAura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    _speedAura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    break;  
            }
            _status = value;
            OnStatusChanged?.Invoke(value);
        }
    }

    IEnumerator WaitForScare()
    {
        yield return new WaitForSeconds(1.0f);
        _source.Stop();
        _source.resource = _fearSound;
        _source.Play();
        _scareAura.Play(true);
        PossessionHandler.Instance.QuitImmediate = true;
    }

    public override int EntityLevel
    {
        get { return _entityLevel; }
        set { _entityLevel = value; }
    }

    public GameObject Torch
    {
        get { return _torch; }
    }

    void Update()
    {
        int index = Array.IndexOf(Enum.GetValues(typeof(GuardStatus)), _status);
        if (_status != GuardStatus.None)
        {
            _progress += Time.deltaTime;
            _progress = Mathf.Clamp(_progress, 0.0f, _statusMaxTime[index]);
            if (_progress == _statusMaxTime[index])
            {
                switch (_status)
                {
                    case GuardStatus.Sleepy:
                        _sleepAura.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        break;
                    case GuardStatus.Scared:
                        _scareAura.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        break;
                    case GuardStatus.Fastened:
                        _speedAura.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                        break;
                }
                OnStatusChanged?.Invoke(GuardStatus.None);
                _status = GuardStatus.None;
            }
        }
    }
}
