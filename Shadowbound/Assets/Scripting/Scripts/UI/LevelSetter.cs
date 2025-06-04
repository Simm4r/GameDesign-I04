using TMPro;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int level;
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (transform.parent.tag)
        {
            case "Possession_Icon":
                level = PlayerStats.Instance.PossessionLevel;
                break;

            case "ShadowVision_Icon":
                level = PlayerStats.Instance.ShadowVisionLevel;
                break;
        }
        _text.text = $"lv.\n{level}";
    }
}
