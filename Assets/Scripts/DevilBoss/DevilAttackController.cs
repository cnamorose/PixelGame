using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilAttackController : MonoBehaviour
{
    List<AttackEntry> attackBag = new List<AttackEntry>();
    int bagIndex = 0;

    [System.Serializable]
    public class AttackEntry
    {
        public MonoBehaviour attack;   // IDevilAttack 구현체
        public float duration;         // 공격 유지 시간

        [Header("Boss Position")]
        public Transform bossPosition; // 공격 시 악마 위치
        public bool hideBoss;
    }

    [Header("Attack List")]
    public List<AttackEntry> attacks;

    [Header("Global Settings")]
    public float intervalBetweenAttacks = 1.5f;

    bool isAttacking = false;
    Coroutine attackRoutine;

    // 현재 실행 중인 공격 추적용
    IDevilAttack currentAttack;
    AttackEntry currentEntry;

    void Start()
    {
        StartCoroutine(StartDelay());
    }
    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2f); // ← 여기 조절

        BeginAttackLoop();
    }
    public void BeginAttackLoop()
    {
        if (attackRoutine != null) return;

        InitAttackBag();
        attackRoutine = StartCoroutine(AttackLoop());
    }

    public void StopAttackLoop()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }

        // 현재 진행 중인 공격도 즉시 종료
        if (currentAttack != null)
        {
            currentAttack.EndAttack();
            currentAttack = null;
            currentEntry = null;
        }

        isAttacking = false;
    }

    AttackEntry GetNextAttack()
    {
        if (attackBag.Count == 0)
            return null;

        if (bagIndex >= attackBag.Count)
            InitAttackBag();

        return attackBag[bagIndex++];
    }

    void InitAttackBag()
    {
        attackBag.Clear();
        attackBag.AddRange(attacks);

        for (int i = attackBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (attackBag[i], attackBag[j]) = (attackBag[j], attackBag[i]);
        }

        bagIndex = 0;
    }

    public void ForceStopAllAttacks()
    {
        StopAttackLoop();

        foreach (var entry in attacks)
        {
            if (entry.attack is IDevilAttack atk)
            {
                atk.EndAttack();
            }
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            if (!gameObject.activeInHierarchy)
                yield break;

            if (!isAttacking)
            {
                isAttacking = true;

                AttackEntry entry = GetNextAttack();
                if (entry == null)
                {
                    Debug.LogError("AttackEntry 없음");
                    attackRoutine = null;
                    yield break;
                }

                IDevilAttack atk = entry.attack as IDevilAttack;
                if (atk == null)
                {
                    Debug.LogError("IDevilAttack 아님");
                    isAttacking = false;
                    yield return null;
                    continue;
                }

                currentEntry = entry;
                currentAttack = atk;

                if (entry.bossPosition != null)
                {
                    transform.position = entry.bossPosition.position;
                    GetComponent<DevilFloat>()?.ResetBasePosition();
                }

                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = !entry.hideBoss;

                atk.StartAttack();

                yield return new WaitForSeconds(entry.duration);

                // 중간에 StopAttackLoop()로 끊겼을 수 있으니 체크
                if (currentAttack == atk)
                {
                    atk.EndAttack();
                    currentAttack = null;
                    currentEntry = null;
                }

                yield return new WaitForSeconds(intervalBetweenAttacks);

                isAttacking = false;
            }

            yield return null;
        }
    }
}