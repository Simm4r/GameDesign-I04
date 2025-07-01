using KinematicCharacterController;
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
            CleanUpAndDestroy();
            return;
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            MarkPersistentObjects();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void MarkPersistentObjects()
    {
        foreach (GameObject obj in _persistentObjects)
        {
            if (obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in _persistentObjects)
        {
            Destroy(obj);
        }
        Destroy(gameObject);
    }

    public void DestroyForScene()
    {
        var kccSystem = FindFirstObjectByType<KinematicCharacterSystem>();
        Destroy(kccSystem);
        CleanUpAndDestroy();
    }
}
