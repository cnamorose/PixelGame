using UnityEngine;

/// <summary>
/// 나사가 들어갈 위치입니다. 나사 중심이 BoxCollider2D 안에 놓이면 스냅됩니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PCScrewSlot : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;

    private BoxCollider2D box;

    public bool IsFilled { get; private set; }

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        SetHighlight(false);
    }

    public bool CanAccept(PCScrewItem item)
    {
        return !IsFilled && item != null;
    }

    public bool IsWithinRange(Vector3 position)
    {
        return box.OverlapPoint(position);
    }

    public void SetHighlight(bool visible)
    {
        if (highlightObject != null)
            highlightObject.SetActive(visible && !IsFilled);
    }

    public void Insert(PCScrewItem item)
    {
        IsFilled = true;
        SetHighlight(false);
        item.SetAsInserted(transform);
    }
}
