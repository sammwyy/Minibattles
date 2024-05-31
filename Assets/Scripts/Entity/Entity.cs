using UnityEngine;
using TMPro;

public class Entity : MonoBehaviour
{
    [Header("State")]
    public bool isControllable = false;

    public int ID { get; private set; }
    public string Label { get; private set; }
    public EntityType EntityType { get; private set; }
    // Entity components.
    public EntityAnimator Animator { get; private set; }
    public EntityController Controller { get; private set; }
    public TextMeshPro TagText { get; private set; }

    public void Init()
    {
        if (Animator == null) Animator = GetComponentInChildren<EntityAnimator>();
        if (Controller == null) Controller = GetComponentInChildren<EntityController>();
        if (TagText == null) TagText = GetComponentInChildren<TextMeshPro>();
    }

    public void Remove()
    {
        Destroy(gameObject);
    }

    public void UpdateIdentity(EntityType entityType, int entityId)
    {
        EntityType = entityType;
        ID = entityId;
        name = $"entity:{ID}";
    }

    public void UpdateLabel(string newLabel)
    {
        Label = newLabel;
        if (TagText)
        {
            TagText.text = newLabel;
        }
    }

    void Awake()
    {
        Init();
    }
}