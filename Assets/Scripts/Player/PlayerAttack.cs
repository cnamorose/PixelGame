using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public GameObject penPrefab;   //Pen 프리팹
    public Transform attackPoint;  // 공격 기준점
    public float stabDistance = 0.3f;
    public float stabDuration = 0.1f;

    PlayerAction action;
    bool isAttacking = false;

    void Awake()
    {
        action = GetComponent<PlayerAction>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "DevilMonster")
            return;

        if (action.forceIdle || isAttacking)
            return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(Stab(action.idleDir));
        }
    }

    IEnumerator Stab(int dir)
    {
        isAttacking = true;
        action.isAttacking = true;

        // Shooter랑 동일: 생성
        GameObject pen = Instantiate(
            penPrefab,
            attackPoint.position,
            Quaternion.identity
        );

        // 방향에 맞게 회전 (수직 스프라이트 기준)
        float angle = dir == 1 ? -90f : 90f;
        pen.transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 startPos = attackPoint.position;
        Vector3 endPos = startPos + Vector3.right * dir * stabDistance;

        float t = 0f;
        while (t < stabDuration)
        {
            pen.transform.position =
                Vector3.Lerp(startPos, endPos, t / stabDuration);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(pen); // 끝나면 제거

        action.isAttacking = false;
        isAttacking = false;
    }
}