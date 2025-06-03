using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Persistent Objects")]
    [SerializeField] private GameObject[] _persistentObjects;
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
