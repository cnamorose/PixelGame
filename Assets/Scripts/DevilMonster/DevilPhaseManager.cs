using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DialogueManager;

public class DevilPhaseManager : MonoBehaviour
{
    [Header("Dialogue")]
    public DialogueSequence phaseEndDialogue;

    [Header("Fade UI")]
    public Image fadePanel;

    [Header("Devil")]
    public GameObject devilObject;


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

        fadePanel.gameObject.SetActive(true);
        fadePanel.color = Color.black;

        yield return new WaitForSeconds(0.3f);

        Color devilRed = new Color(0.2f, 0f, 0f, 1f);
        yield return StartCoroutine(
            FadeColor(Color.black, devilRed, 1.2f)
        );

        devilObject.SetActive(true);

        DevilVisual devil = devilObject.GetComponent<DevilVisual>();
        if (devil != null)
            devil.Show();

        DialogueManager.Instance.currentCutscene =
            DialogueManager.CutsceneType.DevilMonster;

        DialogueManager.Instance.StartDialogue(phaseEndDialogue);
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
