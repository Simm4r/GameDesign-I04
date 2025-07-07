using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private string _tagTrigger = "Player";
    private bool _isEntityInRange = false;
    private GameObject _entity;
    [SerializeField] private bool _isOpen = false;
    [SerializeField] private bool _isAnimationStarted = false;
    [SerializeField] private float _angle = 90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Quaternion _startRotation;
    private Quaternion _finalRotation;
    private float time = 0;
    private float sum = 0;
    [SerializeField] private float _duration = 1.0f;
    [SerializeField] private AudioSource _sound;
    [SerializeField] private AudioClip _open;
    [SerializeField] private AudioClip _close;
    public bool IsAnimationStarted
    {
        get { return _isAnimationStarted; }
    }

    public bool IsOpen
    {
        get => _isOpen;
    }
    void Awake()
    {
        if (_isOpen)
        {
            _startRotation = Quaternion.Euler(Vector3.up * (_angle));
            _finalRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            _startRotation = Quaternion.Euler(Vector3.zero);
            _finalRotation = Quaternion.Euler(Vector3.up * (_angle));
        }

        transform.localRotation = _startRotation;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_isAnimationStarted)
        {
            sum += Time.deltaTime;
            time += Time.deltaTime / _duration;
            transform.localRotation = Quaternion.Slerp(_startRotation, _finalRotation, time);
            if (transform.localRotation == _finalRotation)
            {
                Debug.Log(sum);
                _isOpen = !_isOpen;
                _isAnimationStarted = false;
            }
        }
    }

    public void StartAnimation()
    {
        sum = 0;
        if (_isOpen)
        {
            _startRotation = Quaternion.Euler(Vector3.up * (_angle));
            _finalRotation = Quaternion.Euler(Vector3.zero);
            _sound.resource = _close;
            _sound.Stop();
            _sound.Play();
        }
        else
        {
            _sound.resource = _open;
            _sound.Stop();
            _sound.Play();
            _startRotation = Quaternion.Euler(Vector3.zero);
            _finalRotation = Quaternion.Euler(Vector3.up * (_angle));
        }
        time = 0;
        transform.localRotation = _startRotation;
        _isAnimationStarted = true;
    }

    public void SetOpen()
    {
        StartCoroutine(WaitFade());
    }

    IEnumerator WaitFade()
    {
        yield return new WaitForSeconds(2.0f);
        transform.localRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        _isOpen = true;
    }
}
