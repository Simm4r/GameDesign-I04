using UnityEngine;

public class HandleFlameLit : MonoBehaviour
{
    [SerializeField] private Player _player;
    [SerializeField] private ParticleSystem _flame;
    [SerializeField] private Material _unlitMaterial; 
    [SerializeField] private Material _litMaterial;
    [SerializeField] private Renderer _crystalBall;


    void Update()
    {
        var emission = _flame.emission;
        if (_player.ActualCheckPoint != gameObject && emission.rateOverTime.constant > 0)
        {
            emission.rateOverTime = 0f;
            _crystalBall.material = _unlitMaterial;
            _crystalBall.gameObject.GetComponentInParent<CrystalBall>().CanInteract = true;
        }


        else if (_player.ActualCheckPoint == gameObject && emission.rateOverTime.constant == 0)
        {
            emission.rateOverTime = 40f;
            _crystalBall.material = _litMaterial;
        }
    }
}
