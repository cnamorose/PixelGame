using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedStayTrigger : MonoBehaviour
{
    private float stayTimer = 0f;
    private bool isPlayerInside = false;

    private bool stage1Triggered = false;
    private bool stage2Triggered = false;
    private bool stage3Triggered = false;

    private void Update()
    {
        if (isPlayerInside)
        {
            stayTimer += Time.deltaTime;
            CheckStayTime();
        }
    }

    private void CheckStayTime()
    {
        bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

        // 5초
        if (stayTimer >= 5f && !stage1Triggered)
        {
            Debug.Log("5초 경과!");
            string text = isEN ? "No time to lie down." : "누워있을 시간은 없다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 2f);
            stage1Triggered = true;
        }
        // 15초
        else if (stayTimer >= 15f && !stage2Triggered)
        {
            Debug.Log("15초 경과!");
            string text = isEN ? "I hear the sound of graduation being delayed..." : "졸업이 밀리는 소리가 들린다...";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 2f);
            stage2Triggered = true;
        }
        // 30초
        else if (stayTimer >= 30f && !stage3Triggered)
        {
            Debug.Log("30초 경과!");
            string text = isEN ? "At this rate, I'll be trapped here forever.." : "이러다가 평생 갇혀있겠어..";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 3f);
            stage3Triggered = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 들어오는 순간 로그 찍기
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어 침대 진입!");
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 나가는 순간 로그 찍기
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어 침대 이탈!");
            isPlayerInside = false;
            ResetTimer();
        }
    }

    private void ResetTimer()
    {
        stayTimer = 0f;
        stage1Triggered = false;
        stage2Triggered = false;
        stage3Triggered = false;
    }
}