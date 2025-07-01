using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }
    [SerializeField] private GameObject _logo;
    [SerializeField] GameObject _logoImage;
    [SerializeField] private GameObject _menu;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        if (GameManager.Instance != null)
            GameManager.Instance.DestroyForScene();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Start()
    {
        StartCoroutine(HandleSequence());
    }

    IEnumerator HandleSequence()
    {
        yield return new WaitUntil(() => MenuHandler.Instance != null);
        _menu.SetActive(false);
        ScreenFadeController.Instance.FadeFromBlack();
        _logoImage.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSecondsRealtime(2f);
        _logo.SetActive(false);
        _menu.SetActive(true);
        ScreenFadeController.Instance.FadeFromBlack();
    }
}
