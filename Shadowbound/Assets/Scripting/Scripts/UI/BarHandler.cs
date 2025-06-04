using UnityEngine;

public class BarHandler : MonoBehaviour
{
    public static BarHandler Instance { get; private set; }

    [SerializeField] private GameObject _possessionbar;
    [SerializeField] private GameObject _icons;

    void Awake()
    {
        if (Instance != null)
            return;
        Instance = this;
    }


    void Update()
    {
        if (PlayerInput.Instance.InPossession)
        {
            if (!_possessionbar.activeSelf)
                _possessionbar.SetActive(true);

            if (_icons.activeSelf)
                _icons.SetActive(false);
        }
        else
        {
            if (_possessionbar.activeSelf)
                _possessionbar.SetActive(false);

            if (!_icons.activeSelf)
                _icons.SetActive(true);  
        }
    }
}
