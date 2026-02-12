using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;

    public PlayerData playerData;

    [Header("Posters")]
    public GameObject posterGirl;
    public GameObject posterBoy;

    [Header("Language UI")]
    public GameObject uiKR;
    public GameObject uiEN;

    private string selectedCharacter;

    private void Awake()
    {
        instance = this;

        posterGirl.SetActive(false);
        posterBoy.SetActive(false);

        ApplyLanguageUI();
    }

    void ApplyLanguageUI()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        if (uiKR != null)
            uiKR.SetActive(!isEN);

        if (uiEN != null)
            uiEN.SetActive(isEN);
    }

    public void SelectCharacter(string name)
    {
        Debug.Log("선택 함수 실행: " + name);

        selectedCharacter = name;

        posterGirl.SetActive(name == "Girl");
        posterBoy.SetActive(name == "Boy");
    }

    public void OnClickSelectButton()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.Log("캐릭터 아직 안 고름");
            return;
        }

        playerData.ResetForNewGame();
        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);

        GameObject player = GameManager.instance.SpawnPlayer(selectedCharacter);
        player.SetActive(false);

        AudioManager.Instance.FadeOutAndLoad("school_1", 1.5f);
    }

    private IEnumerator SelectAndLoad()
    {
        playerData.ResetForNewGame();

        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);

        GameObject player = GameManager.instance.SpawnPlayer(selectedCharacter);
        player.SetActive(false);

        // 🎵 BGM 서서히 줄이기
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.FadeOutBGM(1.5f);
        }

        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene("school_1");
    }
}