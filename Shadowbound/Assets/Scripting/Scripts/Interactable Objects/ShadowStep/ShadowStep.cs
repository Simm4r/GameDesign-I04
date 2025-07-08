using System.Collections;
using System.Linq;
using KinematicCharacterController;
using UnityEngine;

public class ShadowStep : Interactable
{
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private InteractionHandler _caller;

    private enum Direction { Left, Right };
    [SerializeField] private Direction _direction;

    private bool _canInteract = false;

    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    public override void Interact()
    {
        _caller.Player = null;
        _canInteract = false;

        StartCoroutine(ShadowTeleportCoroutine());
    }

    IEnumerator ShadowTeleportCoroutine()
    {
        var player = Player.Instance;
        var motor = player.GetComponent<KinematicCharacterMotor>();
        var cam = Camera.main.GetComponent<ThirdPersonCamera>();

        player.InShadowStep = true;

        Renderer rend = player.GetComponentInChildren<Renderer>();
        Outline outline = player.GetComponentInChildren<Outline>();

        var allParticles = player.GetComponentsInChildren<ParticleSystem>(true);
        var trailParticles = allParticles.FirstOrDefault(p => p.gameObject.name == "ShadowStepParticles");
        motor.SetRotation(transform.rotation);
        Transform originalParent = trailParticles.transform.parent;
        trailParticles.transform.parent = null;

        cam.player = trailParticles.transform;
        cam.ForceSetCamera(player.transform.position, -player.transform.forward);
        //cam.SetFollowTarget(trailParticles.transform);

        if (trailParticles != null)
        {
            var emission = trailParticles.emission;
            emission.enabled = true;
            trailParticles.Play();
        }

        float fadeOutDuration = 0.4f;
        float fadeElapsed = 0f;
        while (fadeElapsed < fadeOutDuration)
        {
            fadeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(fadeElapsed / fadeOutDuration);
            float alpha = 1 - t;

            if (rend != null && rend.sharedMaterial.HasProperty("_Alpha"))
                rend.sharedMaterial.SetFloat("_Alpha", alpha);
            if (outline != null)
            {
                Color color = outline.OutlineColor;
                color.a = alpha;
                outline.OutlineColor = color;
            }

            yield return null;
        }

        Vector3 start = trailParticles.transform.position;
        Vector3 end = _exitPoint.position;
        Quaternion endRotation = Quaternion.LookRotation(_exitPoint.forward);

        float moveDuration = 0.4f; // stesso valore del fade
        float moveElapsed = 0f;

        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);
            trailParticles.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        motor.SetPositionAndRotation(end, endRotation);

        float fadeInDuration = 0.4f;
        fadeElapsed = 0f;
        while (fadeElapsed < fadeInDuration)
        {
            fadeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(fadeElapsed / fadeInDuration);
            float alpha = t;

            if (rend != null && rend.sharedMaterial.HasProperty("_Alpha"))
                rend.sharedMaterial.SetFloat("_Alpha", alpha);
            if (outline != null)
            {
                Color color = outline.OutlineColor;
                color.a = alpha;
                outline.OutlineColor = color;
            }

            yield return null;
        }

        if (trailParticles != null)
        {
            var emission = trailParticles.emission;
            emission.enabled = false;
            trailParticles.transform.parent = originalParent;
        }
        cam.player = player.transform;
        //cam.SetFollowTarget(player.transform);
        player.InShadowStep = false;
    }


    void Update()
    {
        if (PlayerInput.Instance.InPossession)
        {
            if (_canInteract)
                _canInteract = false;

            _caller.Player = null;
            return;
        }

        _caller.Player = Player.Instance.gameObject;

        if (!_canInteract)
            _canInteract = true;
    }
}