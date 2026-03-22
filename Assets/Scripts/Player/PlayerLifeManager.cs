using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerLifeManager : MonoBehaviour
{
    public static PlayerLifeManager Instance;
    public event Action OnLifeZero;
    public float invincibleTime = 1f;
    private bool isInvincible = false;

    public int maxLife = 3;
    public int currentLife = 3;

    public event Action OnLifeChanged;
    public Vector3 respawnPosition;

    private Animator animator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        animator = GetComponent<Animator>();
    }

    public void LoseLife()
    {
        if (currentLife <= 0 || isInvincible)
            return;

        currentLife--;

        OnLifeChanged?.Invoke();

        if (currentLife <= 0)
        {
            currentLife = 0;

            // ⭐ 플레이어 숨기기
            SetPlayerVisible(false);

            GameOverManager.Instance.ShowGameOver();
            return;
        }

        StartCoroutine(InvincibleRoutine());
    }

    void SetPlayerVisible(bool visible)
    {
        // 스프라이트 전부 ON/OFF
        var renderers = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var r in renderers)
            r.enabled = visible;

        // 다시 보일 때 애니 초기화
        if (visible && animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.SetFloat("Speed", 0f);
        }
    }

    IEnumerator InvincibleRoutine()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz")
            yield break;

        isInvincible = true;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float time = 0f;
            while (time < 1.5f)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(0.1f);
                time += 0.1f;
            }
            sr.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        isInvincible = false;
    }

    public void FullHeal()
    {
        currentLife = maxLife;
        OnLifeChanged?.Invoke();
    }

    // ⭐ 부활용 (Room 씬에서 한 번 호출)
    public void ShowPlayerAgain()
    {
        SetPlayerVisible(true);
    }
}