using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLanguageSwitcher : MonoBehaviour
{
    [Header("Korean UI")]
    public GameObject startKR;
    public GameObject exitKR;
    public GameObject howtoKR;
    public GameObject titleKR;

    [Header("English UI")]
    public GameObject startEN;
    public GameObject exitEN;
    public GameObject howtoEN;
    public GameObject titleEN;

    void Start()
    {
        ApplyLanguage();
    }

    public void SetKorean()
    {
        GameManager_L.Instance.SetLanguage(Language.KR);
        ApplyLanguage();
    }

    public void SetEnglish()
    {
        GameManager_L.Instance.SetLanguage(Language.EN);
        ApplyLanguage();
    }

    void ApplyLanguage()
    {
        bool isKR = GameManager_L.Instance.currentLanguage == Language.KR;

        startKR.SetActive(isKR);
        exitKR.SetActive(isKR);
        titleKR.SetActive(isKR);

        startEN.SetActive(!isKR);
        exitEN.SetActive(!isKR);
        titleEN.SetActive(!isKR);
    }
}