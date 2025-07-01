using UnityEngine;

[ExecuteAlways]
public class PauseMenuScaler : MonoBehaviour
{
    public Vector2 baseResolution = new Vector2(1920, 1080);
    private Vector2 lastScreenSize;

    void Update()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != lastScreenSize)
        {
            lastScreenSize = currentScreenSize;
            ScaleToScreen();
        }
    }

    void ScaleToScreen()
    {
        float scaleX = Screen.width / baseResolution.x;
        float scaleY = Screen.height / baseResolution.y;
        float scale = Mathf.Min(scaleX, scaleY);
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}