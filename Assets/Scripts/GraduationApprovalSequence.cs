using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GraduationApprovalSequence : MonoBehaviour
{
    // [기존 변수들 그대로 유지]
    [Header("Text")]
    public TextMeshProUGUI nameText;
    public float typingSpeed = 0.08f;

    [Header("Stamp")]
    public GameObject stamp;
    public float stampDelayAfterName = 1f;

    [Header("Camera Shake")]
    public float shakeDuration = 0.5f;
    public float shakePower = 1.0f;

    [Header("Paper Shake")]
    public RectTransform paperRoot;
    public float paperShakePower = 15f;

    [Header("Graduation Photo")]
    public GameObject boyPhoto;
    public GameObject girlPhoto;
    public float photoDropDuration = 1f;
    public float photoStayDuration = 2f;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("END")]
    public TextMeshProUGUI endText;
    public float endStayDuration = 2f;

    [Header("Split Move")]
    public RectTransform creditText;
    public float moveDistance = 300f;
    public float moveDuration = 0.8f;

    [Header("DEBUG")]
    public bool debugMode = true;
    public string debugName = "홍길동";
    public string debugCharacter = "Boy"; // Boy or Girl

    [Header("BGM")]
    public AudioClip phaseBGM;

    [Header("SFX")]
    public AudioClip stpSFX;

    // ⭐ [새로 추가된 종료 버튼 설정]
    [Header("END BUTTON")]
    public GameObject quitButton; // 엔딩 곡이 끝나면 나타날 종료 버튼

    Cameramove cam;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();

        stamp.SetActive(false);
        boyPhoto.SetActive(false);
        girlPhoto.SetActive(false);

        endText.gameObject.SetActive(false);
        creditText.gameObject.SetActive(false);

        // 종료 버튼은 처음에 숨겨둡니다.
        if (quitButton != null)
            quitButton.SetActive(false);

        if (fadePanel != null)
            fadePanel.color = new Color(0, 0, 0, 0);

        StartCoroutine(Sequence());
    }

    IEnumerator ShakeUI(RectTransform target, float duration, float power)
    {
        Vector2 originalPos = target.anchoredPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            Vector2 offset = Random.insideUnitCircle * power;
            target.anchoredPosition = originalPos + offset;
            yield return null;
        }

        target.anchoredPosition = originalPos;
    }

    IEnumerator Sequence()
    {
        // [이름 타이핑 및 도장 쾅 연출 로직 그대로 유지]
        string playerName;

        if (debugMode)
        {
            playerName = debugName;
        }
        else
        {
            playerName = DialogueManager.Instance.playerData.playerName;
        }

        nameText.text = "";

        foreach (char c in playerName)
        {
            nameText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(stampDelayAfterName);

        stamp.SetActive(true);

        if (AudioManager.Instance != null && stpSFX != null)
            AudioManager.Instance.PlayOneShotSFX(stpSFX);

        if (cam != null)
            StartCoroutine(cam.ShakeCameraEnding(shakeDuration, shakePower));

        if (paperRoot != null)
            StartCoroutine(ShakeUI(paperRoot, shakeDuration, paperShakePower));

        yield return new WaitForSeconds(0.5f);

        // ----------------------------------------------------
        // ⭐ BGM 시작 부분 (루프 없이 딱 한 번만 재생)
        // ----------------------------------------------------
        if (AudioManager.Instance != null && phaseBGM != null)
        {
            AudioManager.Instance.PlayBGM(phaseBGM, false);
        }

        // [사진 드롭 및 페이드, 크레딧 연출 로직 그대로 유지]
        string selected;

        if (debugMode)
        {
            selected = debugCharacter;
        }
        else
        {
            selected = PlayerPrefs.GetString("SelectedCharacter", "Boy");
        }

        GameObject activePhoto = selected == "Boy" ? boyPhoto : girlPhoto;

        activePhoto.SetActive(true);

        RectTransform photoRT = activePhoto.GetComponent<RectTransform>();

        Vector2 startPos = new Vector2(0f, 1000f);
        Vector2 endPos = new Vector2(0f, 0f);

        photoRT.anchoredPosition = startPos;

        float t = 0f;
        while (t < photoDropDuration)
        {
            t += Time.deltaTime;
            photoRT.anchoredPosition =
                Vector2.Lerp(startPos, endPos, t / photoDropDuration);
            yield return null;
        }

        yield return new WaitForSeconds(photoStayDuration);

        float f = 0f;
        while (f < fadeDuration)
        {
            f += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, f / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, a);
            yield return null;
        }

        endText.gameObject.SetActive(true);
        endText.alpha = 0;

        float et = 0f;
        while (et < 1f)
        {
            et += Time.deltaTime;
            endText.alpha = et;
            yield return null;
        }

        yield return new WaitForSeconds(endStayDuration);
        yield return new WaitForSeconds(0.5f);

        endText.gameObject.SetActive(false);

        Vector2 photoStart = photoRT.anchoredPosition;
        Vector2 photoEnd = photoStart + Vector2.left * moveDistance;

        creditText.gameObject.SetActive(true);

        Vector2 creditStart = creditText.anchoredPosition + Vector2.right * moveDistance;
        Vector2 creditEnd = creditText.anchoredPosition;

        creditText.anchoredPosition = creditStart;

        float m = 0f;
        while (m < moveDuration)
        {
            m += Time.deltaTime;
            float progress = m / moveDuration;

            photoRT.anchoredPosition =
                Vector2.Lerp(photoStart, photoEnd, progress);

            creditText.anchoredPosition =
                Vector2.Lerp(creditStart, creditEnd, progress);

            yield return null;
        }

        CreditSequence cs = GetComponent<CreditSequence>();
        if (cs != null)
            cs.Play();

        // ----------------------------------------------------
        // ⭐ [핵심 추가] 엔딩 곡이 끝날 때까지 실시간 대기 후 종료 버튼 활성화
        // ----------------------------------------------------
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            // 노래가 플레이 중인 동안에는 계속 양보(대기)합니다.
            while (AudioManager.Instance.bgmSource.isPlaying)
            {
                yield return null;
            }
        }

        // 노래가 완전히 끝나면 종료 버튼을 화면에 띄웁니다!
        if (quitButton != null)
        {
            quitButton.SetActive(true);
        }
    }

    // [버튼 연결용 함수] 게임을 완전히 종료하거나 타이틀로 보낼 때 사용
    public void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}