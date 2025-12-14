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
        }
    }


    public void LoseLife()
    {
        // ⭐ 이미 죽었거나 무적이면 무시
        if (currentLife <= 0 || isInvincible)
            return;

        currentLife--;

        OnLifeChanged?.Invoke();

        if (currentLife <= 0)
        {
            currentLife = 0;
            GameOverManager.Instance.ShowGameOver();
            return;
        }

        StartCoroutine(InvincibleRoutine());
    }

    IEnumerator InvincibleRoutine()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Quiz")
            yield break;

        isInvincible = true;

        //깜빡임 연출
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
}
