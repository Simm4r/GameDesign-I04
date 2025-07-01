using UnityEngine;

public class StopOnStoneTaken : MonoBehaviour
{
    private ParticleSystem _system;
    [SerializeField] private GameObject _stone;
    void Awake()
    {
        _system = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_stone == null)
            gameObject.SetActive(false);
    }


}
