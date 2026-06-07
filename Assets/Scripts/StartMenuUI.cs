using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    [Header("How To Play UI")]
    public GameObject howToPlayUI_KR;
    public GameObject howToPlayUI_EN;

    [Header("Setting UI")]
    public GameObject settingUI_KR; // 한국어 설정창 오브젝트 (배경음 소리...)
    public GameObject settingUI_EN; // 영어 설정창 오브젝트 (BGM, SFX...)

    // ---------------- [게임 설명 기능] ----------------
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

    // ---------------- [설정창 기능 수정] ----------------

    // 설정 버튼을 눌렀을 때 호출 (언어 체크 후 맞는 창 활성화)
    public void ShowSetting()
    {
        // 현재 설정된 언어가 영어인지 확인
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (settingUI_KR != null)
            settingUI_KR.SetActive(!isEN); // 영어 아니면(한국어면) 켜기

        if (settingUI_EN != null)
            settingUI_EN.SetActive(isEN);  // 영어면 켜기
    }

    // 설정창을 닫을 때 호출 (둘 다 안전하게 끄기)
    public void HideSetting()
    {
        if (settingUI_KR != null)
            settingUI_KR.SetActive(false);

        if (settingUI_EN != null)
            settingUI_EN.SetActive(false);
    }
}