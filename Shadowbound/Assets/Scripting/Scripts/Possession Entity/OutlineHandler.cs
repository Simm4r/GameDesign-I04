using UnityEngine;

public class OutlineHandler : MonoBehaviour
{
    [SerializeField] private float _outlineMaxLifetime = 5.0f;
    private Outline _outline;
    private float _outlineLifetime = 0.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_outlineLifetime == _outlineMaxLifetime)
        {
            if (_outline.OutlineWidth != 0)
                UnoutlineEntity();

            return;
        }

        _outlineLifetime += Time.deltaTime;
        _outlineLifetime = Mathf.Clamp(_outlineLifetime, 0.0f, _outlineMaxLifetime);
    }

    public void OutlineEntity(ref GameObject entity)
    {
        if (_outline.OutlineMode == Outline.Mode.OutlineAll)
            return;

        _outline.OutlineMode = Outline.Mode.OutlineAll;
        if (entity.tag.StartsWith("Possessable_"))
        {
            if (entity.GetComponent<EntityStats>().EntityLevel > PlayerStats.Instance.PossessionLevel)
                _outline.OutlineColor = Color.red;
            else
                _outline.OutlineColor = new Color(1, 1, 1, 1);
        }
        else if(entity.CompareTag("Interactable"))
            _outline.OutlineColor = new Color(1f, 0.843f, 0f, 1f);

        _outline.OutlineWidth = 2.0f;
        _outlineLifetime = 0.0f;
    }

    public void UnoutlineEntity()
    {
        _outline.OutlineMode = Outline.Mode.NoOutline;
    }
}
