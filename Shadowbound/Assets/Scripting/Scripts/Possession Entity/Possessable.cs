using UnityEngine;

public class Possessable : MonoBehaviour
{
    [SerializeField] private PossessableCueParticles _possessableCueParticles;
    
    public void ShowPossessableCue()
    {
        if (GetComponentInParent<EntityStats>().EntityLevel > PlayerStats.Instance.PossessionLevel)
            return;

        _possessableCueParticles.ShowPossessableCueParticles();
    }

    public void HidePossessableCue()
    {
        _possessableCueParticles.HidePossessableCueParticles();
    }
}
