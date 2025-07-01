using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class CutsceneVideoPlayer : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RawImage _background;
    [SerializeField] private RenderTexture _texture;
    [SerializeField] private TextMeshProUGUI _text;
    private AsyncOperation preloadOperation;

    void Start()
    {
        _videoPlayer.Prepare(); 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _videoPlayer.prepareCompleted += OnPrepared;
    }

    void OnPrepared(VideoPlayer vp)
    {
        _videoPlayer.prepareCompleted -= OnPrepared;
        _background.color = Color.white;
        _background.texture = _texture;
        _videoPlayer.Play();
        StartCoroutine(StartFading());
    }

    IEnumerator StartFading()
    {
        preloadOperation = SceneManager.LoadSceneAsync("lv1");
        preloadOperation.allowSceneActivation = false;
        yield return new WaitForSeconds(11.5f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        _background.color = Color.black;
        _background.texture = null;
        _videoPlayer.Stop();
        _text.gameObject.SetActive(true);
        ScreenFadeController.Instance.FadeFromBlack();
        yield return new WaitForSeconds(3.5f);
        yield return new WaitUntil(() => preloadOperation.progress >= 0.9f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.5f);
        preloadOperation.allowSceneActivation = true;
    }
}
