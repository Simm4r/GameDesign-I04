using UnityEngine;

public class StoneEnabler : MonoBehaviour
{
    [SerializeField] private ChestSmall _chest;
    [SerializeField] private Item _stone;


    void Update()
    {
        if (_chest.GetState == ChestSmall.ChestSmallState.Opened)
        {
            _stone.enabled = true;
            enabled = false;
        }
    }
}
