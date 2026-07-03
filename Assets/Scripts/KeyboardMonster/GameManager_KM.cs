using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // ⭐ 씬 관리를 위해 추가

public class GameManager_KM : MonoBehaviour
{
    public static GameManager_KM Instance;

    [Header("Part Settings")]
    public int partCount = 0;
    public int requiredParts = 5;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // ⭐ [핵심 추가] 씬이 로드될 때마다 ResetPartsOnSceneLoaded 함수가 자동으로 실행되도록 등록
            SceneManager.sceneLoaded += ResetPartsOnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ⭐ [핵심 추가] 오브젝트가 파괴될 때는 이벤트를 해제해 주어야 메모리 누수가 없습니다.
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetPartsOnSceneLoaded;
    }

    // ⭐ [핵심 추가] 죽어서 재시작하거나 씬이 새로 켜질 때마다 무조건 실행되는 리셋 함수
    private void ResetPartsOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetParts();
    }

    // 부품 카운트를 초기화하는 명시적 함수
    public void ResetParts()
    {
        partCount = 0;
        Debug.Log($"[GameManager_KM] 씬 로드로 인해 부품 카운트가 {partCount}으로 초기화되었습니다.");
    }

    public bool HasAllParts()
    {
        return partCount >= requiredParts;
    }

    public void AddPart()
    {
        partCount++;
        Debug.Log($"[GameManager_KM] 부품 획득! 현재 개수: {partCount}/{requiredParts}");

        if (partCount == requiredParts)
        {
            OnTriggerAllPartsCollected();
        }
    }

    private void OnTriggerAllPartsCollected()
    {
        Debug.Log("[GameManager_KM] 모든 부품 획득 완료! 풀피 보상 및 대사 시퀀스 개시.");

        PlayerLifeManager player = PlayerLifeManager.Instance;
        if (player != null)
        {
            player.FullHeal();
        }

        if (DialogueManager.Instance != null)
        {
            bool isEnglishMode = (GameManager_L.Instance != null && GameManager_L.Instance.currentLanguage == Language.EN);
            string textToDisplay = isEnglishMode
                ? "I've collected all the parts! Now I can finally write it!"
                : "부품을 다 모았다! 이제 작성할 수 있어!";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(textToDisplay, 3f, "#172646");
        }
    }
}