using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThanksHandler : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(GoToMainMenu());
    }

    IEnumerator GoToMainMenu()
    {
        yield return new WaitForSecondsRealtime(5.0f);
        ScreenFadeController.Instance.FadeToBlack();
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MainMenu");
    }
}
