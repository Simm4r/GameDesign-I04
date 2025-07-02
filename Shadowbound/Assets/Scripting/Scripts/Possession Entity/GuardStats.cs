using System;
using System.Collections.Generic;
using UnityEngine;

public class GuardStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    [SerializeField] private GameObject _torch;
    [SerializeField] private ParticleSystem _sleepAura;
    [SerializeField] private ParticleSystem _scareAura;
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
                    _sleepAura.Play(true);
                    break;
                case GuardStatus.Scared:
                    _scareAura.Play(true);
                    break;
            }
            _status = value;
        }
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
                }
                _status = GuardStatus.None;
            }
        }
    }
}
