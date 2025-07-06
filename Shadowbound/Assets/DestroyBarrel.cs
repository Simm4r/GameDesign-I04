using System.Collections;
using UnityEngine;

public class DestroyBarrel : Interactable
{
    [SerializeField] private bool _startAnimation = false;
    [SerializeField] ParticleSystem _smash;
    private bool _canInteract = true;
    public override bool CanInteract { get => _canInteract; set => _canInteract = true; }

    public override void Interact()
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
            Interact();
        }
    }
}
