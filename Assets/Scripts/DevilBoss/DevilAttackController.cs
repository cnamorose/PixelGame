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

    void Start()
    {
        BeginAttackLoop();
    }

    // =========================
    // 외부 제어용 (중요)
    // =========================
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
            isAttacking = false;
        }
    }

    // =========================

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

        // Fisher–Yates Shuffle
        for (int i = attackBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (attackBag[i], attackBag[j]) = (attackBag[j], attackBag[i]);
        }

        bagIndex = 0;
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            if (!isAttacking)
            {
                isAttacking = true;

                AttackEntry entry = GetNextAttack();
                if (entry == null)
                {
                    Debug.LogError("AttackEntry 없음");
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

                // 위치 이동
                if (entry.bossPosition != null)
                {
                    transform.position = entry.bossPosition.position;
                    GetComponent<DevilFloat>()?.ResetBasePosition();
                }

                // 보이기 / 숨기기
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                    sr.enabled = !entry.hideBoss;

                // 공격 시작
                atk.StartAttack();

                yield return new WaitForSeconds(entry.duration);

                // 공격 종료
                atk.EndAttack();

                yield return new WaitForSeconds(intervalBetweenAttacks);

                isAttacking = false;
            }

            yield return null;
        }
    }
}