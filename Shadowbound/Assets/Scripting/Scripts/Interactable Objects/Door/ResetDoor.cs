using System;
using System.Collections;
using UnityEngine;

public class ResetDoor : MonoBehaviour
{
    private DoorOpener _opener;

    void Awake()
    {
        _opener = GetComponent<DoorOpener>();
    }
    void OnEnable()
    {
        StartCoroutine(Subscribe());
    }

    IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => PlayerStats.Instance != null);
        PlayerStats.Instance.OnPlayerDeath += ResetEvent;
    }

    void OnDisable()
    {
        PlayerStats.Instance.OnPlayerDeath -= ResetEvent;
    }

    private void ResetEvent()
    {
        Debug.Log("Sono nel Reset Event");
        if (_opener.IsOpen)
            return;

        _opener.SetOpen();
    }
}
