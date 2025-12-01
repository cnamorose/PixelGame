using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    public string characterName;  // "Girl" or "Boy"

    public float doubleClickTime = 0.25f;
    float lastClickTime = 0;

    private void OnMouseDown()
    {
        Debug.Log(gameObject.name + " 클릭됨");  // ← 일단 이거로 눌리는지부터 확인

        if (Time.time - lastClickTime < doubleClickTime)
        {
            Debug.Log(characterName + " 더블클릭 선택!"); // 디버그용

            PlayerPrefs.SetString("SelectedCharacter", characterName);
            SceneManager.LoadScene("school_1");
        }

        lastClickTime = Time.time;
    }
}

