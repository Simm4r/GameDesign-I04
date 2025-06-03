using UnityEngine;

public class MouseStats : EntityStats
{
    [SerializeField] private int _entityLevel;
    
    public override int EntityLevel
    {
        get { return _entityLevel; }
        set { _entityLevel = value; }
    }
}
