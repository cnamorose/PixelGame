using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_KM : MonoBehaviour
{
    public static GameManager_KM Instance;

    [Header("Part Settings")]
    public int requiredParts = 5;

    // 획득한 부품 목록 (중복 불가)
    private HashSet<string> collectedParts = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += ResetPartsOnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetPartsOnSceneLoaded;
    }

    private void ResetPartsOnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetParts();
    }

    // 부품 목록 초기화
    public void ResetParts()
    {
        collectedParts.Clear();
        Debug.Log("[GameManager_KM] 부품 목록 초기화");
    }

    // 특정 부품을 이미 획득했는지
    public bool HasPart(string partName)
    {
        return collectedParts.Contains(partName);
    }

    // 모든 부품 획득 여부
    public bool HasAllParts()
    {
        return collectedParts.Count >= requiredParts;
    }

    // 부품 획득
    public void AddPart(string partName)
    {
        // 이미 획득한 부품이면 무시
        if (collectedParts.Contains(partName))
        {
            Debug.Log($"[GameManager_KM] 이미 획득한 부품 : {partName}");
            return;
        }

        collectedParts.Add(partName);

        Debug.Log($"[GameManager_KM] {partName} 획득");
        Debug.Log($"[GameManager_KM] 현재 {collectedParts.Count}/{requiredParts}");

        if (HasAllParts())
        {
            OnTriggerAllPartsCollected();
        }
    }

    private void OnTriggerAllPartsCollected()
    {
        Debug.Log("[GameManager_KM] 모든 부품 획득 완료!");

        PlayerLifeManager player = PlayerLifeManager.Instance;
        if (player != null)
        {
            player.FullHeal();
        }

        if (DialogueManager.Instance != null)
        {
            bool isEnglishMode =
                (GameManager_L.Instance != null &&
                 GameManager_L.Instance.currentLanguage == Language.EN);

            string textToDisplay = isEnglishMode
                ? "I've collected all the parts! Now I can finally write it!"
                : "부품을 다 모았다! 이제 작성할 수 있어!";

            DialogueManager.Instance.ShowSimpleDialogueAutoClose(
                textToDisplay,
                3f,
                "#172646");
        }
    }
}