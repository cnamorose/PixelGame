using UnityEngine;

/// <summary>
/// PC 조립 화면에서 특정 부품이 장착될 위치입니다.
/// 같은 GameObject의 BoxCollider2D 영역 안에 부품을 놓으면 장착됩니다.
/// 박스 크기를 슬롯 그림에 맞게 인스펙터에서 조절하세요.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PCAssemblySlot : MonoBehaviour
{
    [SerializeField] private PCAssemblyPartId acceptedPart;
    [SerializeField] private GameObject highlightObject;
    [Tooltip("맞는 부품을 넣었을 때 켜질, 미리 준비해 둔 스프라이트/오브젝트")]
    [SerializeField] private GameObject revealObject;
    [SerializeField] private PCAssemblyManager assemblyManager;

    private BoxCollider2D box;

    public PCAssemblyPartId AcceptedPart => acceptedPart;
    public bool IsFilled { get; private set; }

    private void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;

        if (assemblyManager == null)
            assemblyManager = FindObjectOfType<PCAssemblyManager>();

        SetHighlight(false);
        if (revealObject != null)
            revealObject.SetActive(false);
    }

    public bool CanAccept(PCAssemblyItem item)
    {
        return !IsFilled && item != null && item.PartId == acceptedPart;
    }

    public bool IsWithinSnapRange(Vector3 position)
    {
        // 부품 중심점이 슬롯 박스 영역 안에 있으면 장착 가능.
        return box.OverlapPoint(position);
    }

    public void SetHighlight(bool visible)
    {
        if (highlightObject != null)
            highlightObject.SetActive(visible && !IsFilled);
    }

    public void Place(PCAssemblyItem item)
    {
        IsFilled = true;
        SetHighlight(false);

        // 넣은 아이템은 숨기고, 준비해 둔 스프라이트를 켠다.
        item.HidePlaced();
        if (revealObject != null)
            revealObject.SetActive(true);

        if (assemblyManager != null)
            assemblyManager.PlayPartPlacedSfx();
    }
}
