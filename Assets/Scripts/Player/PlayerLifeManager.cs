using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeManager : MonoBehaviour
{
    public int maxLife = 3;
    public int currentLife = 3;

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환해도 유지
    }

    public void LoseLife()
    {
        if (currentLife <= 0) return;

        currentLife--;
        Debug.Log("Player Life = " + currentLife);
    }

    public void ResetLife()
    {
        currentLife = maxLife;
    }
}
