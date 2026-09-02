using UnityEngine;

/// <summary>
/// 조립 화면에서 드래그할 부품입니다.
/// SpriteRenderer와 BoxCollider2D가 같은 GameObject에 있어야 OnMouse 이벤트를 받을 수 있습니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PCAssemblyItem : MonoBehaviour
{
    [SerializeField] private PCAssemblyPartId partId;
    [SerializeField] private PCAssemblyManager assemblyManager;
    [SerializeField] private int draggingSortingOrder = 100;

    private Vector3 startPosition;
    private Transform startParent;
    private SpriteRenderer spriteRenderer;
    private int defaultSortingOrder;
    private bool isDragging;

    public PCAssemblyPartId PartId => partId;
    public bool IsPlaced { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            defaultSortingOrder = spriteRenderer.sortingOrder;
    }

    private void Start()
    {
        startPosition = transform.position;
        startParent = transform.parent;

        if (assemblyManager == null)
            assemblyManager = FindObjectOfType<PCAssemblyManager>();
    }

    private void OnMouseDown()
    {
        // [진단 로그] 문제 파악 후 이 줄은 지워도 됩니다.
        Debug.Log($"[Assembly] '{name}' 클릭됨. IsPlaced={IsPlaced}, manager={(assemblyManager == null ? "없음!" : "있음")}");

        if (IsPlaced || assemblyManager == null || !assemblyManager.BeginDrag(this))
            return;

        isDragging = true;
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = draggingSortingOrder;
    }

    private void OnMouseDrag()
    {
        if (!isDragging || Camera.main == null)
            return;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        if (!isDragging)
            return;

        isDragging = false;
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = defaultSortingOrder;

        assemblyManager.EndDrag(this);
    }

    /// <summary>
    /// 맞는 슬롯에 넣었을 때 호출됩니다. 아이템을 비활성화해 화면에서 감춥니다.
    /// (대신 슬롯 쪽에서 미리 준비해 둔 스프라이트를 켭니다.)
    /// </summary>
    public void HidePlaced()
    {
        IsPlaced = true;
        gameObject.SetActive(false);
    }

    public void ReturnToStart()
    {
        transform.SetParent(startParent, true);
        transform.position = startPosition;
    }
}
