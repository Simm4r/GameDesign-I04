using UnityEngine;

public class Possessable : MonoBehaviour
{
    [SerializeField] private PossessableCueParticles _possessableCueParticles;

    public void ShowPossessableCue()
    {
        _possessableCueParticles.ShowPossessableCueParticles();
    }

    public void HidePossessableCue()
    {
        _possessableCueParticles.HidePossessableCueParticles();
    }
}
