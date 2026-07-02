using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraduationApprovalSequence : MonoBehaviour
{
    [Header("Text UI (Localized)")]
    public TextMeshProUGUI ko_nameText;
    public TextMeshProUGUI en_nameText;
    public float typingSpeed = 0.08f;

    [Header("Default Label Settings")]
    public TextMeshProUGUI ko_default;
    public TextMeshProUGUI en_default;

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
    public string debugCharacter = "Boy";

    [Header("BGM")]
    public AudioClip phaseBGM;

    [Header("SFX")]
    public AudioClip stpSFX;

    [Header("END BUTTON")]
    public GameObject quitButton;

    [Header("FINAL OBJECTS (여러 개 가능)")]
    public GameObject[] finalAnimationObjects;

    [Header("MILab 2 Persons Cutscene Settings")]
    public TextMeshProUGUI introTitleText;
    public TextMeshProUGUI firstPersonText;
    public GameObject firstPersonCharacter;
    public TextMeshProUGUI secondPersonText;
    public GameObject secondPersonCharacter;
    public float cutsceneDelay = 1.5f;

    // 인스펙터에 적어둔 색상 태그 포함 원본 내용을 저장할 변수
    private string savedIntroText = "";
    private string savedFirstText = "";
    private string savedSecondText = "";

    Cameramove cam;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();

        if (ko_stamp != null) ko_stamp.SetActive(false);
        if (en_stamp != null) en_stamp.SetActive(false);

        boyPhoto.SetActive(false);
        girlPhoto.SetActive(false);

        endText.gameObject.SetActive(false);
        creditText.gameObject.SetActive(false);

        if (ko_default != null) ko_default.gameObject.SetActive(false);
        if (en_default != null) en_default.gameObject.SetActive(false);
        if (ko_nameText != null) ko_nameText.text = "";
        if (en_nameText != null) en_nameText.text = "";

        if (quitButton != null) quitButton.SetActive(false);

        // 🔒 원본 글자(태그 포함)를 백업하고, 눈에 안 보이게 미리 꺼둡니다.
        if (introTitleText != null) { savedIntroText = introTitleText.text; introTitleText.gameObject.SetActive(false); }
        if (firstPersonText != null) { savedFirstText = firstPersonText.text; firstPersonText.gameObject.SetActive(false); }
        if (secondPersonText != null) { savedSecondText = secondPersonText.text; secondPersonText.gameObject.SetActive(false); }

        if (firstPersonCharacter != null) firstPersonCharacter.SetActive(false);
        if (secondPersonCharacter != null) secondPersonCharacter.SetActive(false);

        if (finalAnimationObjects != null)
        {
            foreach (GameObject obj in finalAnimationObjects)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        if (fadePanel != null) fadePanel.color = new Color(0, 0, 0, 0);

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
        bool isEnglishMode = (GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN);

        if (isEnglishMode) { if (en_default != null) en_default.gameObject.SetActive(true); }
        else { if (ko_default != null) ko_default.gameObject.SetActive(true); }

        TextMeshProUGUI targetNameText = isEnglishMode ? en_nameText : ko_nameText;
        string playerName = debugMode ? (isEnglishMode ? debugName_EN : debugName_KR) : DialogueManager.Instance.playerData.playerName;

        if (targetNameText != null)
        {
            targetNameText.text = "";
            foreach (char c in playerName) { targetNameText.text += c; yield return new WaitForSeconds(typingSpeed); }
        }

        yield return new WaitForSeconds(stampDelayAfterName);

        if (isEnglishMode) { if (en_stamp != null) en_stamp.SetActive(true); }
        else { if (ko_stamp != null) ko_stamp.SetActive(true); }

        if (AudioManager.Instance != null && stpSFX != null)
            AudioManager.Instance.PlayOneShotSFX(stpSFX);

        if (cam != null) StartCoroutine(cam.ShakeCameraEnding(shakeDuration, shakePower));
        if (paperRoot != null) StartCoroutine(ShakeUI(paperRoot, shakeDuration, paperShakePower));

        yield return new WaitForSeconds(0.5f);

        if (AudioManager.Instance != null && phaseBGM != null) AudioManager.Instance.PlayBGM(phaseBGM, false);

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
            while (t < photoDropDuration) { t += Time.deltaTime; photoRT.anchoredPosition = Vector2.Lerp(startPos, endPos, t / photoDropDuration); yield return null; }
        }

        yield return new WaitForSeconds(photoStayDuration);

        float f = 0f;
        while (f < fadeDuration) { f += Time.deltaTime; float a = Mathf.Lerp(0f, 1f, f / fadeDuration); fadePanel.color = new Color(0, 0, 0, a); yield return null; }

        if (endText != null) { endText.gameObject.SetActive(true); endText.alpha = 0; }
        float et = 0f;
        while (et < 1f) { et += Time.deltaTime; endText.alpha = et; yield return null; }

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
            while (m < moveDuration) { m += Time.deltaTime; float progress = m / moveDuration; photoRT.anchoredPosition = Vector2.Lerp(photoStart, photoEnd, progress); creditText.anchoredPosition = Vector2.Lerp(creditStart, creditEnd, progress); yield return null; }
        }

        // ----------------------------------------------------
        // 🎬 크레딧 시작 및 대기
        // ----------------------------------------------------
        CreditSequence cs = GetComponent<CreditSequence>();
        if (cs != null)
        {
            cs.isEnded = false;
            cs.Play();
            yield return null;
            while (!cs.isEnded) { yield return null; }
        }

        // ==================================================
        // 🎬 [크레딧 완벽 종료 타이밍] 고급 타이핑 연출 시작
        // ==================================================
        Debug.Log("크레딧 완벽 종료! 안전한 리치 텍스트 타이핑 시작");

        if (finalAnimationObjects != null)
        {
            foreach (GameObject obj in finalAnimationObjects)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        // 0단계. 대타이틀 문구 타이핑 등장 (maxVisibleCharacters 활용)
        if (introTitleText != null && !string.IsNullOrEmpty(savedIntroText))
        {
            introTitleText.text = savedIntroText; // 태그 포함 전체 문장 미리 대입
            introTitleText.maxVisibleCharacters = 0; // 우선 다 숨김
            introTitleText.gameObject.SetActive(true);

            // 텍스트 정보 업데이트 유도
            introTitleText.ForceMeshUpdate();
            int totalCharacters = introTitleText.textInfo.characterCount;

            for (int i = 0; i <= totalCharacters; i++)
            {
                introTitleText.maxVisibleCharacters = i; // 한 글자씩 보이게 함 (태그 자동 제외)
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(cutsceneDelay);
        }

        // 1단계. 첫 번째 사람(교수님) 문구 타이핑 등장
        if (firstPersonText != null && !string.IsNullOrEmpty(savedFirstText))
        {
            firstPersonText.text = savedFirstText;
            firstPersonText.maxVisibleCharacters = 0;
            firstPersonText.gameObject.SetActive(true);

            firstPersonText.ForceMeshUpdate();
            int totalCharacters = firstPersonText.textInfo.characterCount;

            for (int i = 0; i <= totalCharacters; i++)
            {
                firstPersonText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(cutsceneDelay);
        }

        // 2단계. 첫 번째 사람(교수님) 캐릭터 등장
        if (firstPersonCharacter != null)
        {
            firstPersonCharacter.SetActive(true);
            yield return new WaitForSeconds(cutsceneDelay);
        }

        // 3단계. 두 번째 사람(선배님) 문구 타이핑 등장
        if (secondPersonText != null && !string.IsNullOrEmpty(savedSecondText))
        {
            secondPersonText.text = savedSecondText;
            secondPersonText.maxVisibleCharacters = 0;
            secondPersonText.gameObject.SetActive(true);

            secondPersonText.ForceMeshUpdate();
            int totalCharacters = secondPersonText.textInfo.characterCount;

            for (int i = 0; i <= totalCharacters; i++)
            {
                secondPersonText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(cutsceneDelay);
        }

        // 4단계. 두 번째 사람(선배님) 캐릭터 등장
        if (secondPersonCharacter != null)
        {
            secondPersonCharacter.SetActive(true);
            yield return new WaitForSeconds(cutsceneDelay);
        }

        // ----------------------------------------------------
        // 🎵 기존 BGM 종료 대기 및 종료 버튼 활성화
        // ----------------------------------------------------
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