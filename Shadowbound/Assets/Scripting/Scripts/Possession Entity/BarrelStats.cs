using UnityEngine;

public class BarrelStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    
    public override int EntityLevel
    {
        get { return _entityLevel; }
        set { _entityLevel = value; }
    }
}