using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GraduationApprovalSequence : MonoBehaviour
{
    // [요구사항 반영] 영/한 텍스트창 컴포넌트 각각 분리
    [Header("Text UI (Localized)")]
    public TextMeshProUGUI ko_nameText; // 한국어 이름이 타이핑될 텍스트창
    public TextMeshProUGUI en_nameText; // 영어 이름이 타이핑될 텍스트창
    public float typingSpeed = 0.08f;

    // [요구사항 반영] 영/한 디폴트 라벨 UI 컴포넌트 설정
    [Header("Default Label Settings")]
    public TextMeshProUGUI ko_default;  // 상시 띄워둘 한국어 라벨 UI ("성명 : ")
    public TextMeshProUGUI en_default;  // 상시 띄워둘 영어 라벨 UI ("Name : ")

    // [요구사항 반영] 영/한 도장 각각 매핑
    [Header("Stamp (Localized)")]
    public GameObject ko_stamp;
    public GameObject en_stamp;
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
    public string debugName_KR = "홍길동";
    public string debugName_EN = "Gildong Hong";
    public string debugCharacter = "Boy"; // Boy or Girl

    [Header("BGM")]
    public AudioClip phaseBGM;

    [Header("SFX")]
    public AudioClip stpSFX;

    [Header("END BUTTON")]
    public GameObject quitButton;

    Cameramove cam;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();

        // 두 도장 모두 처음에 꺼두기
        if (ko_stamp != null) ko_stamp.SetActive(false);
        if (en_stamp != null) en_stamp.SetActive(false);

        boyPhoto.SetActive(false);
        girlPhoto.SetActive(false);

        endText.gameObject.SetActive(false);
        creditText.gameObject.SetActive(false);

        // ⭐ [초기화 추가] 라벨 및 이름 텍스트창을 언어 체크 전까지 모두 숨기거나 비웁니다.
        if (ko_default != null) ko_default.gameObject.SetActive(false);
        if (en_default != null) en_default.gameObject.SetActive(false);
        if (ko_nameText != null) ko_nameText.text = "";
        if (en_nameText != null) en_nameText.text = "";

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
        // ----------------------------------------------------
        // ⭐ 설정 언어 체크 및 타겟 UI/라벨 엄격 매핑
        // ----------------------------------------------------
        bool isEnglishMode = (GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN);

        // 현재 언어에 맞는 라벨 UI 오브젝트만 활성화 (디폴트로 계속 켜져 있음)
        if (isEnglishMode)
        {
            if (en_default != null) en_default.gameObject.SetActive(true);
        }
        else
        {
            if (ko_default != null) ko_default.gameObject.SetActive(true);
        }

        // 언어에 따라 타이핑을 진행할 진짜 이름 텍스트창 골라내기
        TextMeshProUGUI targetNameText = isEnglishMode ? en_nameText : ko_nameText;

        string playerName = "";

        if (debugMode)
        {
            playerName = isEnglishMode ? debugName_EN : debugName_KR;
        }
        else
        {
            playerName = DialogueManager.Instance.playerData.playerName;
        }

        // 찾아낸 타겟 이름 텍스트창에 원래 의도하신 대로 이름만 한 글자씩 타이핑 효과 연출
        if (targetNameText != null)
        {
            targetNameText.text = ""; // 비우고 시작

            foreach (char c in playerName)
            {
                targetNameText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        yield return new WaitForSeconds(stampDelayAfterName);

        // ----------------------------------------------------
        // ⭐ 언어에 맞는 도장 오브젝트 활성화
        // ----------------------------------------------------
        if (isEnglishMode)
        {
            if (en_stamp != null) en_stamp.SetActive(true);
        }
        else
        {
            if (ko_stamp != null) ko_stamp.SetActive(true);
        }

        // [이하 기존 연출 로직 원본 상태 완벽 유지]
        if (AudioManager.Instance != null && stpSFX != null)
            AudioManager.Instance.PlayOneShotSFX(stpSFX);

        if (cam != null)
            StartCoroutine(cam.ShakeCameraEnding(shakeDuration, shakePower));

        if (paperRoot != null)
            StartCoroutine(ShakeUI(paperRoot, shakeDuration, paperShakePower));

        yield return new WaitForSeconds(0.5f);

        if (AudioManager.Instance != null && phaseBGM != null)
        {
            AudioManager.Instance.PlayBGM(phaseBGM, false);
        }

        string selected = debugMode ? debugCharacter : PlayerPrefs.GetString("SelectedCharacter", "Boy");
        GameObject activePhoto = selected == "Boy" ? boyPhoto : girlPhoto;

        if (activePhoto != null)
        {
            activePhoto.SetActive(true);
            RectTransform photoRT = activePhoto.GetComponent<RectTransform>();
            Vector2 startPos = new Vector2(0f, 1000f);
            Vector2 endPos = new Vector2(0f, 0f);
            photoRT.anchoredPosition = startPos;

            float t = 0f;
            while (t < photoDropDuration)
            {
                t += Time.deltaTime;
                photoRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t / photoDropDuration);
                yield return null;
            }
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

        if (endText != null)
        {
            endText.gameObject.SetActive(true);
            endText.alpha = 0;
        }

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

        if (activePhoto != null)
        {
            RectTransform photoRT = activePhoto.GetComponent<RectTransform>();
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
                photoRT.anchoredPosition = Vector2.Lerp(photoStart, photoEnd, progress);
                creditText.anchoredPosition = Vector2.Lerp(creditStart, creditEnd, progress);
                yield return null;
            }
        }

        CreditSequence cs = GetComponent<CreditSequence>();
        if (cs != null)
            cs.Play();

        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            while (AudioManager.Instance.bgmSource.isPlaying)
            {
                yield return null;
            }
        }

        if (quitButton != null)
        {
            quitButton.SetActive(true);
        }
    }

    public void OnQuitButtonClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}