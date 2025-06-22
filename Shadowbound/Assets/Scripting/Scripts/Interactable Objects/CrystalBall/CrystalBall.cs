using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct RespawnPoint
{
    public string scene;
    public Vector3 position;
    public Quaternion rotation;
}
public class CrystalBall : Interactable
{
    [SerializeField] private GameObject _respawnPosition;
    [SerializeField] private ParticleSystem _flame;
    [SerializeField] private InteractionHandler _caller;
    private bool _canInteract = false;
    public override bool CanInteract
    {
        get { return _canInteract; }
        set { _canInteract = value; }
    }

    public override void Interact()
    {
        _caller.Player = null;
        _canInteract = false;
        RespawnPoint checkPoint;
        checkPoint.position = _respawnPosition.transform.position;
        checkPoint.rotation = _respawnPosition.transform.rotation;
        Debug.Log(checkPoint.position);
        checkPoint.scene = SceneManager.GetActiveScene().name;
        Player.Instance.ActualCheckPoint = checkPoint;
    }

    void Update()
    {
        if (PlayerInput.Instance.InPossession)
        {
            if (_canInteract != false)
                _canInteract = false;

                _caller.Player = null;
            return;
        }

        _caller.Player = Player.Instance.gameObject;
    }
}
