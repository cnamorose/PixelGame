using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerLifeManager : MonoBehaviour
{
    public static PlayerLifeManager Instance;
    public event Action OnLifeZero;

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
        currentLife = Mathf.Max(0, currentLife - 1);
        OnLifeChanged?.Invoke();
        if (currentLife <= 0)
        {
            GameOverManager.Instance.ShowGameOver();
        }
    }

    public void FullHeal()
    {
        currentLife = maxLife;
        OnLifeChanged?.Invoke();
    }
}
