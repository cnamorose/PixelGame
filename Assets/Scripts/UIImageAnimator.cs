using UnityEngine;
using UnityEngine.UI;

public class UIImageAnimator : MonoBehaviour
{
    [Header("애니메이션 스프라이트 목록")]
    public Sprite[] animSprites;

    [Header("프레임 속도 (초 단위)")]
    [SerializeField] private float frameRate = 0.15f;

    [Header("좌우 반전 (체크하면 뒤집힘)")]
    [SerializeField] private bool isFlipX = false; // ⭐ 추가된 체크박스

    private Image targetImage;
    private int currentFrameIndex;
    private float timer;

    void Awake()
    {
        targetImage = GetComponent<Image>();
        ApplyFlip(); // 게임 시작 시 뒤집기 적용
    }

    void OnEnable()
    {
        currentFrameIndex = 0;
        timer = 0f;
        UpdateSprite();
        ApplyFlip(); // 켜질 때도 뒤집기 상태 적용
    }

    void Update()
    {
        if (animSprites == null || animSprites.Length == 0 || targetImage == null) return;

        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrameIndex = (currentFrameIndex + 1) % animSprites.Length;
            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        if (animSprites.Length > currentFrameIndex && animSprites[currentFrameIndex] != null)
        {
            targetImage.sprite = animSprites[currentFrameIndex];
        }
    }

    // ⭐ [추가] 실시간으로 좌우를 뒤집어주는 함수
    private void ApplyFlip()
    {
        Vector3 scale = transform.localScale;

        // 체크박스가 켜져있으면 X축 스케일을 마이너스로, 꺼져있으면 플러스로 고정
        scale.x = isFlipX ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    // ⭐ [추가] 유니티 에디터 인스펙터 창에서 체크박스를 누르자마자 게임 뷰에 바로 반영되게 만듦
    private void OnValidate()
    {
        ApplyFlip();
    }
}