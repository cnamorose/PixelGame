using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectedCharacter : MonoBehaviour
{
    public string characterName;  // "Girl" or "Boy"

    private void OnMouseDown()
    {
        //Debug.Log("캐릭터 클릭됨: " + characterName);
        CharacterSelectManager.instance.SelectCharacter(characterName);
    }
}