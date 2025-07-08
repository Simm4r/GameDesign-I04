using UnityEngine;

public class PanicKillButton : MonoBehaviour
{
    void Update()
    {
        if (PlayerInput.Instance.KillInstant && !PlayerInput.Instance.Dying && PlayerStats.Instance.CanBeHit)
        {
            PlayerStats.Instance.CanBeHit = false;
            PlayerStats.Instance.HitByLaser = true;
        }
    }
}
