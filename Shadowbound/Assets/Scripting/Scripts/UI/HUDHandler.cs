using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    [SerializeField] private GameObject _textBox;
    [SerializeField] private GameObject _cooldownBar;
    void Update()
    {
        if (Player.Instance.InDialogue)
        {
            if (!_textBox.activeSelf)
                _textBox.SetActive(true);

            if (_cooldownBar.activeSelf)
                _cooldownBar.SetActive(false);
        }

        else
        {
            if (_textBox.activeSelf)
                _textBox.SetActive(false);
            
            if (!_cooldownBar.activeSelf)
                _cooldownBar.SetActive(true);
        }
    }
}
