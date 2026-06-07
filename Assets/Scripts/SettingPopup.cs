using UnityEngine;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [Header("볼륨 슬라이더 설정")]
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("깃허브 문의 설정")]
    [SerializeField] private string githubUsername = "cnamorose"; // 내 계정 ID
    [SerializeField] private string repositoryName = "FaceCard"; // 피드백 레포 이름

    void Start()
    {
        // 1. 게임이 켜질 때 오디오 매니저의 현재 볼륨 값을 슬라이더 핸들 위치에 똑같이 맞춰줍니다.
        // 이 노하우 덕분에 한글 창에서 볼륨을 바꾸고 영어 창을 열어도 싱크가 칼같이 맞습니다.
        if (AudioManager.Instance != null)
        {
            if (bgmSlider != null)
                bgmSlider.value = AudioManager.Instance.bgmSource.volume;

            if (sfxSlider != null)
                sfxSlider.value = AudioManager.Instance.sfxSource.volume;
        }

        // 2. 슬라이더 바를 마우스로 밀 때마다 실시간으로 오디오 매니저의 볼륨을 바꾸도록 이벤트를 꽂아줍니다.
        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    // BGM 슬라이더 조작 시 호출
    private void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
    }

    // SFX 슬라이더 조작 시 호출
    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    // [버튼 연결용] 깃허브 문의하기 (현재 언어 모드를 체크해서 알맞은 양식 팝업)
    public void OpenGithubFeedback()
    {
        string title = "";
        string body = "";

        // GameManager_L의 현재 언어 세팅 상태 확인
        if (GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN)
        {
            // 영문 이슈 템플릿
            title = EscapeURL("[Bug/Suggestion] One-line summary");
            body = EscapeURL(
                "## 📌 What is the issue?\n" +
                "Please describe your issue freely.\n\n" +
                "## 💻 Environment\n" +
                "- OS (Windows/Mac/Etc): \n" +
                "- Details: \n\n" +
                "## 📸 Screenshots (Optional)\n" +
                "Drag and drop images here if any."
            );
        }
        else
        {
            // 국문 이슈 템플릿 (기본값)
            title = EscapeURL("[버그/건의] 한 줄 요약을 적어주세요");
            body = EscapeURL(
                "## 📌 어떤 문제가 있나요?\n" +
                "내용을 자유롭게 적어주세요.\n\n" +
                "## 💻 실행 환경\n" +
                "- OS (Windows/Mac/기타): \n" +
                "- 특이 사항: \n\n" +
                "## 📸 스크린샷 (선택)\n" +
                "이미지가 있다면 여기에 드래그 앤 드롭 해주세요."
            );
        }

        // 브라우저로 띄울 깃허브 New Issue 주소 조립
        string githubUrl = $"https://github.com/{githubUsername}/{repositoryName}/issues/new?title={title}&body={body}";

        // 유저 기본 웹 브라우저 열기
        Application.OpenURL(githubUrl);
    }

    // [버튼 연결용] 게임 종료 기능
    public void QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터 창에서는 재생 모드를 꺼줌
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 PC 게임 exe 환경에서는 프로세스를 완전 종료
        Application.Quit();
#endif
    }

    // URL에 한글이나 공백이 들어가도 깨지지 않게 변환해 주는 편의 함수
    private string EscapeURL(string url)
    {
        return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
    }
}