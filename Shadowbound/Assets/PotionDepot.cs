using System;
using UnityEngine;

public class PotionDepot : MonoBehaviour
{
    public enum PotionType
    {
        Red,
        Blue,
        Green
    }

    [SerializeField] private PotionType _type;
    
}
