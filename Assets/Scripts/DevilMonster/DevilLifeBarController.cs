using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilLifeBarController : MonoBehaviour
{

    [Header("Phase End Dialogue")]
    public DialogueSequence devilPhase1EndDialogue;

    [Header("Boss HP")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Life Bar Sprites")]
    public Sprite hp100Sprite; // 100 ~ 71
    public Sprite hp70Sprite;  // 70 ~ 41
    public Sprite hp40Sprite;  // 40 ~ 11
    public Sprite hp10Sprite;  // 10 ~ 1
    public Sprite hp0Sprite;   // 0

    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
        UpdateLifeBar();
    }

    public void ReduceHP(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateLifeBar();
    }

    void UpdateLifeBar()
    {
        if (currentHP > 70)
        {
            sr.sprite = hp100Sprite;
        }
        else if (currentHP > 40)
        {
            sr.sprite = hp70Sprite;
        }
        else if (currentHP > 10)
        {
            sr.sprite = hp40Sprite;
        }
        else if (currentHP > 0)
        {
            sr.sprite = hp10Sprite;
        }
        else
        {
            sr.sprite = hp0Sprite;
            OnBossPhaseEnd();
        }
    }

    void ClearAllMonsters()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject m in monsters)
        {
            Destroy(m);
        }
    }

    void OnBossPhaseEnd()
    {
        Debug.Log("Devil Phase 1 종료");

        ClearAllMonsters();

        MonsterSpawner spawner = FindObjectOfType<MonsterSpawner>();
        if (spawner != null)
            spawner.enabled = false;

        StartCoroutine(DevilPhaseEndSequence());
    }

    IEnumerator DevilPhaseEndSequence()
    {
        // ▶ 플레이어 조작 잠금
        PlayerAction player = FindObjectOfType<PlayerAction>();
        if (player != null)
            player.LockControl();

        // ▶ 컷신 타입 지정
        DialogueManager.Instance.currentCutscene =
            DialogueManager.CutsceneType.KeyMonster;
        // 또는 DevilPhase 같은 enum 추가해도 됨

        // ▶ 대사 시작
        DialogueManager.Instance.StartDialogue(devilPhase1EndDialogue);

        yield return null;
    }
}
