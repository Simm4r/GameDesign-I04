using System;
using System.Collections;
using UnityEngine;

public class StatBoost : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(WaitForSub());

    }
    IEnumerator WaitForSub()
    {
        yield return new WaitUntil(() => StartCuscene.Instance != null);
        StartCuscene.Instance.OnCutsceneEnd += IncreasePossessionLevel;
    }

    void OnDisable()
    {
        StartCuscene.Instance.OnCutsceneEnd -= IncreasePossessionLevel;
    }

    private void IncreasePossessionLevel()
    {
        PossessionHandler.Instance.PossessionMaxTime = 30.0f;
        PlayerStats.Instance.PossessionLevel = 3;
        PossessionHandler.Instance.PossessionMaxCooldown = 7.0f;
    }
}
