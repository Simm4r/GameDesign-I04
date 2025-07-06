using System.Collections;
using UnityEngine;

public class DestroyBarrel : MonoBehaviour
{
    [SerializeField] private bool _startAnimation = false;
    [SerializeField] ParticleSystem _smash;

    public void Destroybarrel()
    {
        if (!PlayerInput.Instance.InPossession && !PlayerInput.Instance.QuitPossession)
        {
            _smash.gameObject.transform.SetParent(null);
            _smash.Play();
            Destroy(gameObject);
        }

        if (PlayerInput.Instance.InPossession && PossessionHandler.Instance.PossessedEntity == gameObject)
        {
            PossessionHandler.Instance.QuitImmediate = true;
            StartCoroutine(WaitForPossessionQuit());
        }
    }

    IEnumerator WaitForPossessionQuit()
    {
        yield return new WaitUntil(() => !PlayerInput.Instance.InPossession && !PossessionHandler.Instance.ChoosingPosition && !UndissolveController.Instance.IsUndissolving);
        _smash.gameObject.transform.SetParent(null);
        _smash.Play();
        Destroy(gameObject);
    }

    void Update()
    {
        if (_startAnimation)
        {
            _startAnimation = false;
            Destroybarrel();
        }
    }
}
