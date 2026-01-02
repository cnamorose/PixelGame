using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public Transform attackPoint;      // 손 위치
    public SpriteRenderer penRenderer; // Pen의 SpriteRenderer
    public float attackDuration = 0.15f;
    public float swingAngle = 130f;

    PlayerAction action;
    Coroutine swingRoutine;

    void Awake()
    {
        action = GetComponent<PlayerAction>();
        penRenderer.enabled = false; // ⭐ 처음엔 안 보이게
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "DevilMonster")
            return;

        if (action.forceIdle)
            return;

        if (Input.GetKeyDown(KeyCode.A))
            StartSwing(true);

        if (Input.GetKeyDown(KeyCode.D))
            StartSwing(false);
    }

    void StartSwing(bool isLeft)
    {
        if (swingRoutine != null)
            StopCoroutine(swingRoutine);

        swingRoutine = StartCoroutine(Swing(isLeft));
    }

    IEnumerator Swing(bool isLeft)
    {
        action.isAttacking = true;
        penRenderer.enabled = true;

        // 🔹 위치 (플레이어 바로 앞)
        attackPoint.localPosition = isLeft
            ? new Vector3(-0.25f, 0f, 0f)
            : new Vector3(0.25f, 0f, 0f);

        float startAngle = 60f;
        float endAngle = -60f;

        attackPoint.localRotation = Quaternion.Euler(0, 0, startAngle);

        float t = 0f;
        while (t < attackDuration)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, t / attackDuration);
            attackPoint.localRotation = Quaternion.Euler(0, 0, angle);
            t += Time.deltaTime;
            yield return null;
        }

        attackPoint.localRotation = Quaternion.identity;
        penRenderer.enabled = false;
        action.isAttacking = false;
    }
}