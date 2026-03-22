using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DialogueManager;

public class DevilPhaseManager : MonoBehaviour
{
    [Header("BGM")]
    public AudioClip phaseBGM;

    [Header("Dialogue")]
    public DialogueSequence bossDeathDialogue;
    public DialogueSequence bossDeathDialogue_EN;

    [Header("Fade UI")]
    public Image fadePanel;

    [Header("Devil")]
    public GameObject devilObject;

    [Header("Next Scene")]
    public string nextSceneName = "Stage2";

    public void Start()
    {
        if (AudioManager.Instance != null && phaseBGM != null)
        {
            AudioManager.Instance.PlayBGM(phaseBGM);
        }
    }

    public void StartPhaseEnd()
    {
        StartCoroutine(PhaseEndSequence());
    }

    IEnumerator PhaseEndSequence()
    {
        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null)
            player.LockControl();

        foreach (var m in GameObject.FindGameObjectsWithTag("Monster"))
            Destroy(m);

        MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>();
        if (spawner != null)
            spawner.enabled = false;

        yield return new WaitForSeconds(1f);

        // 이미 1->2 대화를 본 적 있으면 바로 다음 씬으로
        if (GameProgressManager.Instance != null &&
            GameProgressManager.Instance.hasSeenPhase1EndDialogue)
        {
            if (fadePanel != null)
            {
                fadePanel.gameObject.SetActive(true);
                fadePanel.color = Color.black;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopBGM();
            }

            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(nextSceneName);
            yield break;
        }

        // 처음 보는 경우만 아래 연출 + 대화 실행
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = Color.black;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        yield return new WaitForSeconds(1f);

        Color devilRed = new Color(0.2f, 0f, 0f, 1f);
        yield return StartCoroutine(FadeColor(Color.black, devilRed, 1.2f));

        if (devilObject != null)
            devilObject.SetActive(true);

        DevilVisual devil = devilObject != null ? devilObject.GetComponent<DevilVisual>() : null;
        if (devil != null)
            devil.Show();

        DialogueManager.Instance.currentCutscene =
            DialogueManager.CutsceneType.DevilMonster;

        DialogueSequence selectedDialogue =
            GameManager_L.Instance.currentLanguage == Language.EN &&
            bossDeathDialogue_EN != null
                ? bossDeathDialogue_EN
                : bossDeathDialogue;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBGM();
        }

        // 대화 본 상태 저장
        if (GameProgressManager.Instance != null)
        {
            GameProgressManager.Instance.hasSeenPhase1EndDialogue = true;
        }

        DialogueManager.Instance.StartDialogue(selectedDialogue);
    }

    IEnumerator FadeColor(Color from, Color to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadePanel.color = Color.Lerp(from, to, t / duration);
            yield return null;
        }
        fadePanel.color = to;
    }
}