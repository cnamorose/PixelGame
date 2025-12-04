using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;

    [Header("Posters")]
    public GameObject posterGirl;   // poG
    public GameObject posterBoy;    // poB

    private string selectedCharacter;

    private void Awake()
    {
        instance = this;

        // 시작할 땐 포스터 숨김
        posterGirl.SetActive(false);
        posterBoy.SetActive(false);
    }

    public void SelectCharacter(string name)
    {
        Debug.Log("선택 함수 실행: " + name);

        selectedCharacter = name;

        // 해당 캐릭터 포스터만 켜기
        posterGirl.SetActive(name == "Girl");
        posterBoy.SetActive(name == "Boy");
    }

    // 버튼에서 호출할 함수
    public void OnClickSelectButton()
    {
        if (string.IsNullOrEmpty(selectedCharacter))
        {
            Debug.Log("캐릭터 아직 안 고름");
            return;
        }

        PlayerPrefs.SetString("SelectedCharacter", selectedCharacter);
        GameObject player = GameManager.instance.SpawnPlayer(selectedCharacter);
        player.SetActive(false);
        SceneManager.LoadScene("school_1");
    }
}
