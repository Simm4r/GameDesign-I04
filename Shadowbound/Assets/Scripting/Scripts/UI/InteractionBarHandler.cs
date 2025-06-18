using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionBarHandler : MonoBehaviour
{
    public static InteractionBarHandler Instance { get; private set; }

    [SerializeField] private GameObject[] _slots;
    [SerializeField] private List<InteractionHandler> _interactions;
    private Dictionary<GameObject, InteractionHandler> _objectAndInteractions = new();
    [SerializeField] private GameObject _interactSprite;
    [SerializeField] private GameObject _upSprite;
    [SerializeField] private GameObject _downSprite;
    [SerializeField] private GameObject _higlightBar;
    private bool _canInteract = false;
    private int _actualSlot = 0;

    [SerializeField] private List<string> _debugDictionary = new();

    private void PrintDictionary()
    {
        _debugDictionary.Clear();
        foreach (var kvp in _objectAndInteractions)
        {
            string keyName = kvp.Key != null ? kvp.Key.name : "null";
            string valueName = kvp.Value != null ? kvp.Value.ToString() : "null";
            _debugDictionary.Add($"{keyName} => {valueName}");
        }
    }
    private void Awake()
    {
        if (Instance != null)
            return;

        Instance = this;
    }

    public void AddInteraction(InteractionHandler interaction)
    {
        if (interaction == null)
            return;

        if (!_interactions.Contains(interaction))
        {
            _interactions.Add(interaction);
            var index = _interactions.IndexOf(interaction);
            if (!_objectAndInteractions.ContainsKey(_slots[index]))
            {
                _objectAndInteractions.Add(_slots[index], interaction);
                PrintDictionary();
            }
                
        }
    }

    public void RemoveInteraction(InteractionHandler interaction)
    {
        if (interaction == null)
            return;

        if (_interactions.Contains(interaction))
        {
            var index = _interactions.IndexOf(interaction);
            _interactions.Remove(interaction);
            _objectAndInteractions.Remove(_slots[index]);
            PrintDictionary();
        }
    }

     void Update()
    {
        if (!_canInteract)
            return;
        RectTransform keys = _interactSprite.transform.parent.GetComponent<RectTransform>();
        RectTransform higlight = _higlightBar.GetComponent<RectTransform>();

        if (PlayerInput.Instance.InteractUp && _actualSlot > 0)
        {
            _actualSlot--;
            float yPoint = _slots[_actualSlot].GetComponent<RectTransform>().position.y;
            ChangeActiveSlot(yPoint, keys, higlight);
        }
        if (PlayerInput.Instance.InteractDown && _actualSlot < _interactions.Count - 1)
        {
            _actualSlot++;
            float yPoint = _slots[_actualSlot].GetComponent<RectTransform>().position.y;
            ChangeActiveSlot(yPoint, keys, higlight);
        }
        if (PlayerInput.Instance.Interact)
        {
            _interactions[_actualSlot].Interact();
        }
    }

    private void LateUpdate()
    {
        UpdateUI();
        if (_interactions.Count > 0 && !_interactSprite.activeSelf)
        {
            _interactSprite.SetActive(true);
            _higlightBar.SetActive(true);
        }
        else if (_interactions.Count == 0 && _interactSprite.activeSelf)
        {
            _interactSprite.SetActive(false);
            _higlightBar.SetActive(false);
        }

        if (_interactions.Count > 1 && (!_upSprite.activeSelf || !_downSprite.activeSelf))
        {
            _upSprite.SetActive(true);
            _downSprite.SetActive(true);
        }
        else if (_interactions.Count < 1 && (_upSprite.activeSelf || _downSprite.activeSelf))
        {
            _upSprite.SetActive(false);
            _downSprite.SetActive(false);
        }
        RectTransform keys = _interactSprite.transform.parent.GetComponent<RectTransform>();
        RectTransform higlight = _higlightBar.GetComponent<RectTransform>();
        
        if (_interactions.Count == 0)
        {
            _canInteract = false;
            _actualSlot = 0;
            float yPoint = _slots[_interactions.Count].GetComponent<RectTransform>().position.y;
            ChangeActiveSlot(yPoint, keys, higlight);
            return;
        }
        if (_actualSlot >= _interactions.Count)
        {
            _actualSlot = _interactions.Count - 1;
            float yPoint = _slots[_actualSlot].GetComponent<RectTransform>().position.y;
            ChangeActiveSlot(yPoint, keys, higlight);
        }
        _canInteract = true;
    }

    private void UpdateUI()
    {
        int i = 0;
        for (i = 0; i < _interactions.Count; i++)
        {
            var interaction = _interactions[i];
            TextMeshProUGUI text;
            if (!_slots[i].activeSelf)
            {
                _slots[i].SetActive(true);
                text = _slots[i].GetComponentInChildren<TextMeshProUGUI>();
                text.text = _interactions[i].Text;
            }

            text = _slots[i].GetComponentInChildren<TextMeshProUGUI>();

            text.text = interaction.Text;
        }

        while (i < _slots.Count())
        {
            if(_slots[i].activeSelf)
                _slots[i].SetActive(false);
            i++;
        }
    }

    private void ChangeActiveSlot(float yPoint, RectTransform keys, RectTransform higlight)
    {
        keys.position = new Vector3(keys.position.x, yPoint, keys.position.y);
        higlight.position = new Vector3(higlight.position.x, yPoint, higlight.position.z);
    }
}
