using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using KinematicCharacterController;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad;
    [SerializeField] private string _tag;
    [SerializeField] private Animator _fadeAnim;
    [SerializeField] private float _fadeTime;
    [SerializeField] private float _minCheckDistance = 2.0f;
    [SerializeField] private Vector3 _boxExtents;
    [SerializeField] private Vector3 _newPlayerposition;

    void Update()
    {
        if (Vector3.Distance(Player.Instance.transform.position, transform.position) > _minCheckDistance)
            return;

        Collider[] hits = Physics.OverlapBox(transform.position, _boxExtents, Quaternion.identity);
        hits = hits.Where(h => h.gameObject.tag == _tag).ToArray();

        if (hits.Length == 0)
            return;

        _fadeAnim.Play("FadeIn");
        StartCoroutine(DelayFade());
    }

    IEnumerator DelayFade()
    {
        yield return new WaitForSeconds(_fadeTime);
        KinematicCharacterMotor motor = Player.Instance.GetComponent<KinematicCharacterMotor>();
        motor.SetPosition(_newPlayerposition);
        SceneManager.LoadScene(_sceneToLoad);
    }
}
