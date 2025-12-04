using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject playerPrefab;
    private GameObject currentPlayer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 씬이 로드될 때 player 위치만 옮겨줌 (새로 생성 안함)
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentPlayer == null) return;

        currentPlayer.SetActive(true);

        GameObject spawn = GameObject.Find("PlayerPoint");
        if (spawn != null)
            currentPlayer.transform.position = spawn.transform.position;
    }

    // 최초로 플레이어 생성하는 함수
    public GameObject SpawnPlayer(string characterType)
    {
        // 기존 플레이어 제거
        if (currentPlayer != null)
            Destroy(currentPlayer);

        // 새 플레이어 생성
        currentPlayer = Instantiate(playerPrefab);
        DontDestroyOnLoad(currentPlayer);

        // 캐릭터 타입 적용
        PlayerAction pa = currentPlayer.GetComponent<PlayerAction>();
        pa.SetCharacter(characterType);

        return currentPlayer;
    }
}
