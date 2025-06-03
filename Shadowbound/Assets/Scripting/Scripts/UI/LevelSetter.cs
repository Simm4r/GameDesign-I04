using TMPro;
using UnityEngine;

public class LevelSetter : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
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
                level = _stats.PossessionLevel;
                break;

            case "ShadowVision_Icon":
                level = _stats.ShadowVisionLevel;
                break;
        }
        _text.text = $"lv.\n{level}";
    }
}
