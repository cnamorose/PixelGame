using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    KR,
    EN
}

public class GameManager_L : MonoBehaviour
{
    public static GameManager_L Instance;
    public Language currentLanguage = Language.KR;

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

    public void SetLanguage(Language lang)
    {
        currentLanguage = lang;
    }
}