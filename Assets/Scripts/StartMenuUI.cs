using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    [Header("How To Play UI")]
    public GameObject howToPlayUI_KR;
    public GameObject howToPlayUI_EN;

    public void Show()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (howToPlayUI_KR != null)
            howToPlayUI_KR.SetActive(!isEN);

        if (howToPlayUI_EN != null)
            howToPlayUI_EN.SetActive(isEN);
    }

    public void Hide()
    {
        if (howToPlayUI_KR != null)
            howToPlayUI_KR.SetActive(false);

        if (howToPlayUI_EN != null)
            howToPlayUI_EN.SetActive(false);
    }
}