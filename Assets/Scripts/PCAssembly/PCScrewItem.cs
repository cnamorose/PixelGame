using UnityEngine;

/// <summary>
/// 바닥에 있는 나사를 드래그해 슬롯에 넣고, 시계방향으로 돌려 체결합니다.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PCScrewItem : MonoBehaviour
{
    [SerializeField] private PCScrewManager screwManager;
    [SerializeField, Min(1)] private int requiredTurns = 3;
    [SerializeField] private int draggingSortingOrder = 110;
    [Header("Placed Screw Visual")]
    [SerializeField, Range(0.1f, 1f)] private float insertedScaleMultiplier = 0.6f;
    [SerializeField, Range(0f, 0.5f)] private float scaleReductionPerTurn = 0.12f;

    private Vector3 startPosition;
    private Transform startParent;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private int defaultSortingOrder;
    private float lastMouseAngle;
    private float clockwiseDegrees;
    private Vector3 insertedBaseScale;
    private PCScrewTurnGuide turnGuide;
    private AudioSource turnAudioSource;
    private float lastTurnMotionTime;
    private bool isDragging;
    private bool isTurning;

    public bool IsInserted { get; private set; }
    public bool IsTightened { get; private set; }
    public int TurnsCompleted => Mathf.Clamp(Mathf.FloorToInt(clockwiseDegrees / 360f), 0, requiredTurns);
    public int RequiredTurns => requiredTurns;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        turnAudioSource = GetComponent<AudioSource>();
        if (turnAudioSource == null)
            turnAudioSource = gameObject.AddComponent<AudioSource>();
        turnAudioSource.playOnAwake = false;
        turnAudioSource.loop = true;
        turnAudioSource.spatialBlend = 0f;

        if (spriteRenderer != null)
            defaultSortingOrder = spriteRenderer.sortingOrder;
    }

    private void Start()
    {
        startPosition = transform.position;
        startParent = transform.parent;

        if (screwManager == null)
            screwManager = FindObjectOfType<PCScrewManager>();
    }

    private void OnMouseDown()
    {
        if (IsTightened || screwManager == null)
            return;

        if (!IsInserted)
        {
            if (!screwManager.BeginDrag(this))
                return;

            isDragging = true;
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = draggingSortingOrder;
            return;
        }

        BeginTurning();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            if (Camera.main == null)
                return;

            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
            return;
        }

        ContinueTurning();
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = defaultSortingOrder;
            screwManager.EndDrag(this);
        }

        EndTurning();
    }

    private void Update()
    {
        // 마우스를 누른 채 멈췄을 때도 회전음이 계속 나지 않도록 끊는다.
        if (turnAudioSource != null && turnAudioSource.isPlaying
            && Time.unscaledTime - lastTurnMotionTime > 0.08f)
        {
            turnAudioSource.Stop();
        }
    }

    public void SetAsInserted(Transform slotTransform)
    {
        IsInserted = true;
        isDragging = false;
        isTurning = false;
        clockwiseDegrees = 0f;
        transform.SetParent(slotTransform, true);
        transform.position = slotTransform.position;

        // 바닥에서 사용한 나사 종류와 상관없이, 슬롯 안에서는 공통 체결 나사로 보입니다.
        if (spriteRenderer != null && screwManager != null && screwManager.InstalledScrewSprite != null)
        {
            spriteRenderer.sprite = screwManager.InstalledScrewSprite;
            if (boxCollider != null)
                boxCollider.size = spriteRenderer.sprite.bounds.size;
        }

        insertedBaseScale = transform.localScale * insertedScaleMultiplier;
        transform.localScale = insertedBaseScale;

        GameObject guideObject = new GameObject("ScrewTurnGuide");
        guideObject.transform.SetParent(slotTransform, false);
        guideObject.transform.position = transform.position;
        turnGuide = guideObject.AddComponent<PCScrewTurnGuide>();
        turnGuide.Initialize(this);
    }

    public void ReturnToStart()
    {
        transform.SetParent(startParent, true);
        transform.position = startPosition;
    }

    private float GetMouseAngle()
    {
        if (Camera.main == null)
            return 0f;

        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouse - transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private void Tighten()
    {
        if (IsTightened)
            return;

        IsTightened = true;
        isTurning = false;
        StopTurnAudio();
        if (turnGuide != null)
            Destroy(turnGuide.gameObject);
        screwManager.NotifyTightened(this);
    }

    private void UpdateTighteningScale()
    {
        // 1회전마다 줄어드는 비율을 적용하되, 회전 중에도 부드럽게 축소합니다.
        float completedTurnProgress = Mathf.Min(clockwiseDegrees / 360f, requiredTurns);
        float scaleMultiplier = Mathf.Pow(1f - scaleReductionPerTurn, completedTurnProgress);
        transform.localScale = insertedBaseScale * scaleMultiplier;
        if (turnGuide != null)
            turnGuide.UpdateProgress(TurnsCompleted, requiredTurns);
    }

    public void BeginTurning()
    {
        if (!IsInserted || IsTightened)
            return;

        isTurning = true;
        lastMouseAngle = GetMouseAngle();
    }

    public void ContinueTurning()
    {
        if (!isTurning || IsTightened)
            return;

        float currentMouseAngle = GetMouseAngle();
        float angleDelta = Mathf.DeltaAngle(lastMouseAngle, currentMouseAngle);
        lastMouseAngle = currentMouseAngle;

        if (Mathf.Abs(angleDelta) > 0.01f)
            PlayTurnAudio();

        // 마우스가 시계 방향으로 움직일 때 angleDelta는 음수입니다.
        transform.Rotate(0f, 0f, angleDelta);
        if (angleDelta < 0f)
        {
            clockwiseDegrees += -angleDelta;
            UpdateTighteningScale();
        }

        if (clockwiseDegrees >= requiredTurns * 360f)
            Tighten();
    }

    public void EndTurning()
    {
        isTurning = false;
        StopTurnAudio();
    }

    private void PlayTurnAudio()
    {
        if (turnAudioSource == null || screwManager == null || screwManager.ScrewTurnSfx == null)
            return;

        if (turnAudioSource.clip != screwManager.ScrewTurnSfx)
            turnAudioSource.clip = screwManager.ScrewTurnSfx;

        if (!turnAudioSource.isPlaying)
            turnAudioSource.Play();

        lastTurnMotionTime = Time.unscaledTime;
    }

    private void StopTurnAudio()
    {
        if (turnAudioSource != null && turnAudioSource.isPlaying)
            turnAudioSource.Stop();
    }
}
