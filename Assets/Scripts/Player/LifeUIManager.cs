using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUIManager : MonoBehaviour
{
    public Image[] lifePills;
    public Sprite fullPill;
    public Sprite emptyPill;

    void OnEnable()
    {
        PlayerLifeManager.Instance.OnLifeChanged += UpdateLifeUI;
    }

    void OnDisable()
    {
        PlayerLifeManager.Instance.OnLifeChanged -= UpdateLifeUI;
    }

    void Start()
    {
        UpdateLifeUI();
    }

    public void UpdateLifeUI()
    {
        int life = PlayerLifeManager.Instance.currentLife;

        for (int i = 0; i < lifePills.Length; i++)
        {
            lifePills[i].sprite = (i < life) ? fullPill : emptyPill;
        }
    }
}
