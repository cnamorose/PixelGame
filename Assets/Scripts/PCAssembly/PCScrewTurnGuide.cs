using UnityEngine;

/// <summary>
/// 슬롯에 배치된 나사 주변에 큰 회전 영역과 진행 안내를 표시합니다.
/// 나사 자체가 아니라 이 원형 영역을 드래그해도 체결할 수 있습니다.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class PCScrewTurnGuide : MonoBehaviour
{
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float rotationSpeed = 80f;

    private PCScrewItem screw;
    private LineRenderer ring;
    private TextMesh progressText;

    public void Initialize(PCScrewItem targetScrew)
    {
        screw = targetScrew;

        CircleCollider2D clickArea = GetComponent<CircleCollider2D>();
        clickArea.isTrigger = true;
        clickArea.radius = radius * 1.25f;

        CreateRing();
        CreateProgressText();
        UpdateProgress(screw.TurnsCompleted, screw.RequiredTurns);
    }

    private void Update()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }

    private void OnMouseDown()
    {
        if (screw != null)
            screw.BeginTurning();
    }

    private void OnMouseDrag()
    {
        if (screw != null)
            screw.ContinueTurning();
    }

    private void OnMouseUp()
    {
        if (screw != null)
            screw.EndTurning();
    }

    public void UpdateProgress(int completedTurns, int requiredTurns)
    {
        if (progressText != null)
            progressText.text = "TURN " + completedTurns + "/" + requiredTurns;
    }

    private void CreateRing()
    {
        ring = gameObject.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = false;
        ring.positionCount = 30;
        ring.widthMultiplier = 0.035f;
        ring.startColor = new Color(1f, 0.82f, 0.15f, 0.95f);
        ring.endColor = ring.startColor;
        ring.material = new Material(Shader.Find("Sprites/Default"));
        ring.sortingOrder = 200;

        // 약간 비워 둔 원과 화살표 끝으로 시계 방향 회전을 표현합니다.
        for (int i = 0; i < ring.positionCount; i++)
        {
            float angle = Mathf.Lerp(35f, -290f, i / (float)(ring.positionCount - 1));
            float radians = angle * Mathf.Deg2Rad;
            ring.SetPosition(i, new Vector3(Mathf.Cos(radians) * radius, Mathf.Sin(radians) * radius, 0f));
        }

        float endRadians = -290f * Mathf.Deg2Rad;
        Vector3 tip = new Vector3(Mathf.Cos(endRadians) * radius, Mathf.Sin(endRadians) * radius, 0f);
        Vector3 clockwiseTangent = new Vector3(Mathf.Sin(endRadians), -Mathf.Cos(endRadians), 0f);
        Vector3 back = -clockwiseTangent.normalized * 0.14f;

        LineRenderer arrowHead = gameObject.AddComponent<LineRenderer>();
        arrowHead.useWorldSpace = false;
        arrowHead.positionCount = 3;
        arrowHead.widthMultiplier = 0.055f;
        arrowHead.startColor = ring.startColor;
        arrowHead.endColor = ring.startColor;
        arrowHead.material = ring.material;
        arrowHead.sortingOrder = 201;
        arrowHead.SetPosition(0, tip + Quaternion.Euler(0f, 0f, 32f) * back);
        arrowHead.SetPosition(1, tip);
        arrowHead.SetPosition(2, tip + Quaternion.Euler(0f, 0f, -32f) * back);
    }

    private void CreateProgressText()
    {
        GameObject textObject = new GameObject("TurnProgress");
        textObject.transform.SetParent(transform, false);
        textObject.transform.localPosition = new Vector3(0f, radius + 0.18f, 0f);

        progressText = textObject.AddComponent<TextMesh>();
        progressText.anchor = TextAnchor.MiddleCenter;
        progressText.alignment = TextAlignment.Center;
        progressText.characterSize = 0.07f;
        progressText.fontSize = 32;
        progressText.color = new Color(1f, 0.9f, 0.35f, 1f);

        MeshRenderer meshRenderer = textObject.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 201;
    }
}
