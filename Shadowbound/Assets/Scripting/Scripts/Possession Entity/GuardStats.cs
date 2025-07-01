using UnityEngine;

public class GuardStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    [SerializeField] private GameObject _torch;
    public enum GuardStatus
    {
        Sleepy,
        Scared,
        Fastened,
        None
    }

    private GuardStatus _status = GuardStatus.None;
    public GuardStatus Status
    {
        get => _status;
        set => _status = value;
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
}
