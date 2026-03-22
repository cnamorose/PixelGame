using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraduationApprovalSequence : MonoBehaviour
{
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

    Cameramove cam;

    void Start()
    {
        cam = Camera.main.GetComponent<Cameramove>();

        stamp.SetActive(false);
        boyPhoto.SetActive(false);
        girlPhoto.SetActive(false);

        endText.gameObject.SetActive(false);
        creditText.gameObject.SetActive(false);

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

        if (AudioManager.Instance != null && phaseBGM != null)
        {
            AudioManager.Instance.PlayBGM(phaseBGM);
        }

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
    }
}
