using UnityEngine;

public class Possessable : MonoBehaviour
{
    [SerializeField] private PossessableCueParticles _possessableCueParticles;
    [SerializeField] private PlayerStats _playerStats;
    
    public void ShowPossessableCue()
    {
        if (GetComponentInParent<EntityStats>().EntityLevel > _playerStats.PossessionLevel)
            return;

        _possessableCueParticles.ShowPossessableCueParticles();
    }

    public void HidePossessableCue()
    {
        _possessableCueParticles.HidePossessableCueParticles();
    }
}
