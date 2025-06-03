using UnityEngine;

public abstract class EntityStats : MonoBehaviour, IEntityStats
{
    public abstract int EntityLevel { get; set; }
}
