using UnityEngine;

public class GuardStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    [SerializeField] private GameObject _torch;

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
