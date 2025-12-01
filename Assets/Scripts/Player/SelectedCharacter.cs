using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedCharacter : MonoBehaviour
{
    public string characterName; // "Boy" or "Girl"

    float lastClick = 0f;
    float doubleClickTime = 0.25f;

    void OnMouseDown()
    {
        if (Time.time - lastClick < doubleClickTime)
        {
            // 선택된 캐릭터 저장
            PlayerPrefs.SetString("SelectedCharacter", characterName);

            GameManager.instance.SpawnPlayer(characterName);

            // 선택용 캐릭터 즉시 숨기기
            HideAllSelectorCharacters();

            // 다음 씬 로드
            SceneManager.LoadScene("school_1");
        }

        lastClick = Time.time;
    }

    void HideAllSelectorCharacters()
    {
        GameObject[] selectors = GameObject.FindGameObjectsWithTag("Selector");
        foreach (var s in selectors)
            s.SetActive(false);
    }

}
