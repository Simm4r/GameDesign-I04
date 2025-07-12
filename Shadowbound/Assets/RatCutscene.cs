using System;
using System.Collections;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.AI;

public class RatCutscene : Interactable
{
    private bool _canInteract = false;
    private bool _cutsceneStarted = false;
    public override bool CanInteract
    {
        get => _canInteract;
        set => _canInteract = value;
    }

    [SerializeField] private InteractionHandler _caller;
    [SerializeField] private FleeingEntity _rat;
    [SerializeField] private Transform Momo1;
    [SerializeField] private Transform Rat1;
    [SerializeField] private Transform Rat2;
    [SerializeField] private ParticleSystem RatSoul;
    [SerializeField] private Transform SoulPos1;
    [SerializeField] private Transform SoulPos2;
    private GameObject camera1;
    [SerializeField] private GameObject camera2;

    void Start()
    {
        camera1 = Camera.main.gameObject;
    }
    public override void Interact()
    {
        _cutsceneStarted = true;
        _canInteract = false;
        _caller.Player = null;
        StartCoroutine(Cutscene());
    }


    void Update()
    {
        if (_cutsceneStarted || PlayerInput.Instance.InPossession || _rat.CurrentState != FleeingEntity.EntityState.Safe)
        {
            _caller.Player = null;
            _canInteract = false;
            return;
        }

        _caller.Player = Player.Instance.gameObject;
        _canInteract = true;
    }

    IEnumerator Cutscene()
    {
        Player.Instance.InCutscene = true;
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.0f);
        camera1.SetActive(false);
        camera2.SetActive(true);
        Player.Instance.GetComponent<KinematicCharacterMotor>().SetPositionAndRotation(Momo1.position, Momo1.rotation);
        var animator = Player.Instance.GetComponentInChildren<Animator>();
        animator.Play("Blend Tree", 0, 0f);
        animator.speed = 0.0f;
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSeconds(0.7f);
        _rat.gameObject.transform.Find("Rat").gameObject.SetActive(true);
        var agent = _rat.GetComponent<NavMeshAgent>();
        agent.enabled = true;
        _rat.ResetAgent();
        _rat.enabled = false;
        _rat.transform.SetPositionAndRotation(Rat1.position, Rat1.rotation);
        agent.SetDestination(Rat2.position);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);
        float duration = 0.5f;
        float elapsed = 0f;
        Quaternion startRotation = _rat.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(Momo1.position - Rat2.position);
        while (elapsed <= duration)
        {
            elapsed += Time.deltaTime;
            _rat.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }
        RatSoul.transform.position = SoulPos1.position;
        float progress = 0.0f;
        float soulSpeed = 1.0f;
        yield return new WaitForSeconds(0.5f);
        RatSoul.Play(true);
        bool killed = false;
        var ratAnimator = _rat.GetComponent<Animator>();
        while (progress < 1.0f)
        {
            progress = Mathf.Clamp01(progress += Time.deltaTime * soulSpeed);
            var actualPos = Vector3.Lerp(SoulPos1.position, SoulPos2.position, progress);
            RatSoul.transform.position = actualPos;
            if (progress > 0.5f && !killed)
            {
                killed = true;
                ratAnimator.Play("Death");
                Destroy(_rat.GetComponent<ResetEntity>());
                Destroy(_rat);
            }
            yield return null;
        }

        RatSoul.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        yield return new WaitForSeconds(3.0f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.0f);
        camera2.SetActive(false);
        camera1.SetActive(true);
        animator.speed = 1;
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSeconds(1.5f);
        Player.Instance.InCutscene = false;
    }
}
