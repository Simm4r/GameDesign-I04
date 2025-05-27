using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] private int _entityLevel;

    public int EntityLevel => _entityLevel;
}
