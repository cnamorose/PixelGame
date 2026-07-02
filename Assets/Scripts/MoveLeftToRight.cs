using UnityEngine;

public class MoveLeftToRight : MonoBehaviour
{
    [Header("이동 속도")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("UI 오브젝트인가요? (Canvas 내부 사물이면 체크)")]
    [SerializeField] private bool isUI = false;

    private RectTransform rectTransform;

    void Awake()
    {
        // UI 오브젝트일 경우를 대비해 RectTransform 캐싱
        if (isUI)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (isUI && rectTransform != null)
        {
            // ⭐ [수정] Canvas 안의 UI 오브젝트를 왼쪽(Vector2.left)으로 이동
            rectTransform.anchoredPosition += Vector2.left * (moveSpeed * Time.deltaTime);
        }
        else
        {
            // ⭐ [수정] 일반 2D/3D 게임 월드 사물을 왼쪽(Vector3.left)으로 이동
            transform.Translate(Vector3.left * (moveSpeed * Time.deltaTime), Space.World);
        }
    }
}