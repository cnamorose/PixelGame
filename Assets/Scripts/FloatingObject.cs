using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("둥둥거리는 높이 (폭)")]
    [SerializeField] private float floatAmplitude = 15f;

    [Header("둥둥거리는 속도")]
    [SerializeField] private float floatSpeed = 2f;

    [Header("UI 오브젝트인가요? (Canvas 내부 사물이면 체크)")]
    [SerializeField] private bool isUI = false;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;
    private Vector3 startWorldPos;

    void Start()
    {
        if (isUI)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                startAnchoredPos = rectTransform.anchoredPosition;
            }
        }
        else
        {
            startWorldPos = transform.position;
        }
    }

    void Update()
    {
        // 시간을 기반으로 부드러운 사인(Sin) 곡선 값 계산
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        if (isUI && rectTransform != null)
        {
            // UI 오브젝트 위아래로 흔들기
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startAnchoredPos.y + newY);
        }
        else
        {
            // 일반 2D 월드 사물 위아래로 흔들기
            transform.position = new Vector3(transform.position.x, startWorldPos.y + newY, transform.position.z);
        }
    }
}