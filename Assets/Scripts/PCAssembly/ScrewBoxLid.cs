using System.Collections;
using UnityEngine;

/// <summary>
/// 나사 상자 뚜껑을 지정한 목표 위치와 스케일까지 열어 줍니다.
/// Open Target은 렌더러 없는 빈 GameObject로 만들고, 원하는 열린 상태에 배치하세요.
/// </summary>
public class ScrewBoxLid : MonoBehaviour
{
    [SerializeField] private Transform openTarget;
    [SerializeField, Min(0.01f)] private float openDuration = 0.35f;
    [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Header("SFX")]
    [SerializeField] private AudioClip openSfx;

    private Vector3 closedLocalPosition;
    private Vector3 closedLocalScale;
    private Coroutine openRoutine;

    private void Awake()
    {
        closedLocalPosition = transform.localPosition;
        closedLocalScale = transform.localScale;
    }

    public void Open()
    {
        if (openTarget == null)
        {
            Debug.LogWarning("[ScrewBoxLid] Open Target이 연결되지 않았습니다.", this);
            return;
        }

        if (openRoutine != null)
            StopCoroutine(openRoutine);

        if (openSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShotSFX(openSfx);

        openRoutine = StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < openDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / openDuration);
            float curvedProgress = openCurve.Evaluate(progress);

            transform.localPosition = Vector3.Lerp(startPosition, openTarget.localPosition, curvedProgress);
            transform.localScale = Vector3.Lerp(startScale, openTarget.localScale, curvedProgress);
            yield return null;
        }

        transform.localPosition = openTarget.localPosition;
        transform.localScale = openTarget.localScale;
        openRoutine = null;
    }

    public void ResetClosedState()
    {
        if (openRoutine != null)
            StopCoroutine(openRoutine);

        transform.localPosition = closedLocalPosition;
        transform.localScale = closedLocalScale;
        openRoutine = null;
    }
}
