using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleFlameLit : MonoBehaviour
{
    [SerializeField] private ParticleSystem _flame;
    [SerializeField] private Material _unlitMaterial; 
    [SerializeField] private Material _litMaterial;
    [SerializeField] private Renderer _crystalBall;


    void Update()
    {
        var emission = _flame.emission;
        if (
            (Player.Instance.ActualCheckPoint.position != transform.position
            || Player.Instance.ActualCheckPoint.scene != SceneManager.GetActiveScene().name)
            && emission.rateOverTime.constant > 0
        )
        {
            emission.rateOverTime = 0f;
            _crystalBall.material = _unlitMaterial;
            _crystalBall.gameObject.GetComponentInParent<CrystalBall>().CanInteract = true;
        }


        else if (
            Player.Instance.ActualCheckPoint.position == transform.position
            || Player.Instance.ActualCheckPoint.scene == SceneManager.GetActiveScene().name
            && emission.rateOverTime.constant > 0
        )
        {
            emission.rateOverTime = 40f;
            _crystalBall.material = _litMaterial;
        }
    }
}
