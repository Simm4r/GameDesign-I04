using UnityEngine;

public class MoveListener : MonoBehaviour
{
    public static MoveListener Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }
    
    void Update()
    {
        if (!Camera.main)
            return;
        var _player = Camera.main.GetComponent<ThirdPersonCamera>().player;
        if (_player == null)
            return;
        transform.SetPositionAndRotation(_player.transform.position, _player.transform.rotation);
    }
}
