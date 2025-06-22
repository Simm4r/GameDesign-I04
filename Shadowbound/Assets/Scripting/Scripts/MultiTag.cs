using System.Collections.Generic;
using UnityEngine;

public class MultiTag : MonoBehaviour
{
    [SerializeField] private List<string> tags;

    public bool HasTag(string tag) => tags.Contains(tag);
}