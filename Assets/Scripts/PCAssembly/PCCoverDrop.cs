using System.Collections;
using UnityEngine;

/// <summary>
/// PC 커버를 시작 위치에서 지정한 목표 위치까지 아래로 내려 보냅니다.
/// Drop Target은 렌더러 없는 빈 GameObject로 만들고 원하는 최종 위치에 배치하세요.
/// </summary>
public class PCCoverDrop : MonoBehaviour
{
    [SerializeField] private Transform dropTarget;
    [SerializeField, Min(0.01f)] private float dropDuration = 0.45f;
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Header("SFX")]
    [SerializeField] private AudioClip closeSfx;

    private Coroutine dropRoutine;

    public bool IsFinished { get; private set; }

    public void Drop()
    {
        if (IsFinished || dropRoutine != null)
            return;

        if (dropTarget == null)
        {
            Debug.LogWarning("[PCCoverDrop] Drop Target이 연결되지 않았습니다.", this);
            IsFinished = true;
            return;
        }

        if (closeSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShotSFX(closeSfx);

        dropRoutine = StartCoroutine(DropRoutine());
    }

    private IEnumerator DropRoutine()
    {
        Vector3 startPosition = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / dropDuration);
            transform.localPosition = Vector3.Lerp(
                startPosition,
                dropTarget.localPosition,
                dropCurve.Evaluate(progress));
            yield return null;
        }

        transform.localPosition = dropTarget.localPosition;
        IsFinished = true;
        dropRoutine = null;
    }
}
