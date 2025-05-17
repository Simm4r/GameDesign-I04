using UnityEngine;



public class Possession : MonoBehaviour
{
    [SerializeField] private DissolveController dissolveController;
    [SerializeField] private ParticleSystem flameRing;

    private PlayerInput input;
    private bool isPossessing = false;
    private ShadowDamageHandler shadowHandler;

    public bool IsPossessing {
        get { return isPossessing; }
        set { isPossessing = value; }
    }   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        dissolveController = GetComponent<DissolveController>();
        shadowHandler = GetComponent<ShadowDamageHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!shadowHandler.CurrentPossessable)
            return;

        if(input.Possessing) {
            isPossessing = true;
            if (dissolveController != null)
                dissolveController.StartDissolve();

            if (flameRing != null)
            {
                flameRing.Clear();
                flameRing.Play();
            }
        }
    }
}
