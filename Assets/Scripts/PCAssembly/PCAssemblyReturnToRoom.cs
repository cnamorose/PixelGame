using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// PC 전원이 켜진 뒤 조립 완료 상태를 저장하고 방 씬으로 돌아간다.
/// PCAssemblyManager의 On Assembly Complete 이벤트에 연결해 사용한다.
/// </summary>
public class PCAssemblyReturnToRoom : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private string roomSceneName = "Room";
    [SerializeField, Min(0f)] private float afterPowerOnDelay = 1f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 0.45f;
    [SerializeField, Min(0f)] private float fadeInDuration = 1.2f;

    private bool hasReturned;

    public void CompleteAndReturnToRoom()
    {
        if (hasReturned) return;
        hasReturned = true;

        if (playerData != null)
            playerData.pcCleared = true;

        // 씬을 로드해도 Room 페이드 인 코루틴이 끊기지 않도록
        // 전환 담당 오브젝트만 독립 루트로 만든 뒤 유지한다.
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        StartCoroutine(ReturnToRoomRoutine());
    }

    private System.Collections.IEnumerator ReturnToRoomRoutine()
    {
        // 전원이 켜진 PC 화면을 잠시 보여 준 뒤 씬을 전환한다.
        yield return new WaitForSecondsRealtime(afterPowerOnDelay);

        Image fader = CreateTransitionFader();

        yield return Fade(fader, 0f, 1f, fadeOutDuration);
        SceneManager.LoadScene(roomSceneName);

        // 새 Room 씬의 첫 프레임부터 검정 화면을 유지한 뒤 천천히 밝힌다.
        yield return null;
        yield return Fade(fader, 1f, 0f, fadeInDuration);

        Destroy(fader.transform.root.gameObject);

        if (DialogueManager.Instance != null)
        {
            bool isEnglish = GameManager_L.Instance != null
                && GameManager_L.Instance.currentLanguage == Language.EN;
            string message = isEnglish
                ? "The PC is assembled! Now I can write the thesis!"
                : "PC 조립을 완성했다! 이제 논문을 쓸 수 있어!";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(message, 3f);
        }

        Destroy(gameObject);
    }

    private static Image CreateTransitionFader()
    {
        GameObject faderObject = new GameObject("PCSceneTransitionFader");
        Canvas canvas = faderObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        faderObject.AddComponent<CanvasScaler>();
        faderObject.AddComponent<GraphicRaycaster>();

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(faderObject.transform, false);
        Image image = imageObject.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        image.raycastTarget = false;

        RectTransform rect = image.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        DontDestroyOnLoad(faderObject);
        return image;
    }

    private static System.Collections.IEnumerator Fade(
        Image image, float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            image.color = new Color(0f, 0f, 0f, to);
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            image.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        image.color = new Color(0f, 0f, 0f, to);
    }
}
