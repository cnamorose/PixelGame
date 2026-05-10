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
        // 현재 목숨과 최대 목숨을 가져옵니다.
        int currentLife = PlayerLifeManager.Instance.currentLife;
        int maxLife = PlayerLifeManager.Instance.maxLife;

        for (int i = 0; i < lifePills.Length; i++)
        {
            // ⭐ 이 줄이 중요합니다! 
            // 배열 인덱스가 maxLife보다 작으면 활성화(Show), 크거나 같으면 비활성화(Hide)
            lifePills[i].gameObject.SetActive(i < maxLife);

            // 보여지는 알약이라면, 채워진 상태인지 빈 상태인지 결정
            if (i < maxLife)
            {
                lifePills[i].sprite = (i < currentLife) ? fullPill : emptyPill;
            }
        }
    }
}
