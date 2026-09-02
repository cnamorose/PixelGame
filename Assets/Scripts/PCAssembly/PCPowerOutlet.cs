using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 나사 체결 완료 후 나타나는 콘센트입니다.
/// 클릭하면 전원이 켜진 스프라이트로 바뀌고 이후 단계를 진행시킵니다.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class PCPowerOutlet : MonoBehaviour
{
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;
    [Tooltip("전원을 켰을 때 함께 보여 줄 스프라이트 오브젝트입니다. 예: PC LED 또는 켜진 화면")]
    [SerializeField] private GameObject powerOnVisual;
    [SerializeField, Min(0f)] private float powerOnVisualDelay = 1f;
    [SerializeField] private UnityEvent onPowerOn;
    [Header("SFX")]
    [SerializeField] private AudioClip powerOnSfx;

    private SpriteRenderer spriteRenderer;
    private bool isUnlocked;

    public bool IsPowered { get; private set; }
    public bool IsPowerOnVisualReady { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ShowOffSprite();
        if (powerOnVisual != null)
            powerOnVisual.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Unlock()
    {
        if (isUnlocked)
            return;

        isUnlocked = true;
        gameObject.SetActive(true);
        ShowOffSprite();
    }

    private void OnMouseDown()
    {
        if (!isUnlocked || IsPowered)
            return;

        IsPowered = true;
        if (spriteRenderer != null && onSprite != null)
            spriteRenderer.sprite = onSprite;

        if (powerOnSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShotSFX(powerOnSfx);

        onPowerOn?.Invoke();
        StartCoroutine(ShowPowerOnVisual());
    }

    private System.Collections.IEnumerator ShowPowerOnVisual()
    {
        yield return new WaitForSecondsRealtime(powerOnVisualDelay);

        if (powerOnVisual != null)
            powerOnVisual.SetActive(true);

        IsPowerOnVisualReady = true;
    }

    private void ShowOffSprite()
    {
        if (spriteRenderer != null && offSprite != null)
            spriteRenderer.sprite = offSprite;
    }
}
