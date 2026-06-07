using UnityEngine;

public class SettingToggleUI : MonoBehaviour
{
    [Header("언어별 설정창 오브젝트")]
    public GameObject settingUI_KR; // 한국어 설정창 (배경음 소리...)
    public GameObject settingUI_EN; // 영어 설정창 (BGM, SFX...)

    // 다른 씬의 설정 버튼(톱니바퀴 등)이 클릭되었을 때 호출할 함수
    public void ShowSetting()
    {
        // GameManager_L에서 현재 언어 상태를 가져옴
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (settingUI_KR != null)
            settingUI_KR.SetActive(!isEN); // 한국어면 켜기

        if (settingUI_EN != null)
            settingUI_EN.SetActive(isEN);  // 영어면 켜기
    }

    // 설정창의 X 버튼을 눌렀을 때 호출할 함수
    public void HideSetting()
    {
        if (settingUI_KR != null)
            settingUI_KR.SetActive(false);

        if (settingUI_EN != null)
            settingUI_EN.SetActive(false);
    }
}