using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUIManager : MonoBehaviour
{
    public PlayerLifeManager playerLife;
    public Image[] lifePills;
    public Sprite fullPill;
    public Sprite emptyPill;

    void Start()
    {
        playerLife = FindObjectOfType<PlayerLifeManager>();
        UpdateLifeUI();
    }

    public void UpdateLifeUI()
    {
        for (int i = 0; i < lifePills.Length; i++)
        {
            if (i < playerLife.currentLife)
                lifePills[i].sprite = fullPill;
            else
                lifePills[i].sprite = emptyPill;
        }
    }
}
