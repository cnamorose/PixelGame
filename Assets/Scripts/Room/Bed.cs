using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    private float stayTimer = 0f;      // 이스터에그용 타이머
    private float recoveryTimer = 0f;  // 체력 회복용 타이머 (1초 간격)
    private float sleepDelayTimer = 0f; // 잠들기 전 대기 타이머 (2초)
    private bool isPlayerInside = false;

    // --- 플래그 관리 ---
    private bool didShowSleepMsg = false;   // "잠들어버렸다" 출력 여부
    private bool isRecovering = false;      // 현재 회복 중인지 여부
    private bool didShowWakeUpMsg = false;  // "이제 일어나자" 출력 여부

    private bool didShow5s = false;
    private bool didShow15s = false;
    private bool didShow30s = false;

    private void Update()
    {
        // 1. 거리 계산 방식
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);

            if (dist < 1.0f)
            {
                if (!isPlayerInside)
                {
                    Debug.Log("<color=cyan>침대 범위 진입!</color>");
                    isPlayerInside = true;
                }
            }
            else
            {
                if (isPlayerInside)
                {
                    Debug.Log("<color=yellow>침대 범위 이탈!</color>");
                    OnExitBed();
                }
            }
        }

        // 2. 침대 안에서의 로직
        if (isPlayerInside)
        {
            var life = PlayerLifeManager.Instance;
            if (life == null) return;

            bool isEN = GameManager_L.Instance.currentLanguage == Language.EN;

            // --- A. 체력이 부족한 경우 (회복 시퀀스) ---
            if (life.currentLife < life.maxLife)
            {
                // 기상 대사 플래그 초기화 (나중에 풀피 되면 다시 띄우기 위해)
                didShowWakeUpMsg = false;

                // 잠들기 전 2초 대기
                if (!isRecovering)
                {
                    sleepDelayTimer += Time.deltaTime;
                    if (sleepDelayTimer >= 2f && !didShowSleepMsg)
                    {
                        string sleepText = isEN ? "I fell asleep..." : "잠들어버렸다..";
                        DialogueManager.Instance.ShowSimpleDialogueAutoClose(sleepText, 2f);
                        didShowSleepMsg = true;
                        isRecovering = true; // 이제부터 초당 회복 시작
                    }
                }
                // 잠든 후 초당 1씩 회복
                else
                {
                    recoveryTimer += Time.deltaTime;
                    if (recoveryTimer >= 1f)
                    {
                        life.currentLife++;
                        life.CallOnLifeChanged();
                        recoveryTimer = 0f;
                        Debug.Log("체력 1 회복됨");
                    }
                }

                // 회복 중에는 이스터에그 타이머가 가지 않도록 초기화
                stayTimer = 0f;
            }
            // --- B. 체력이 꽉 찬 경우 ---
            else
            {
                // 방금 막 회복을 마쳤다면 "이제 일어나자" 출력
                if (isRecovering && !didShowWakeUpMsg)
                {
                    string wakeUpText = isEN ? "Now, let's wake up." : "몸이 가벼워졌다. 이제 일어나자.";
                    DialogueManager.Instance.ShowSimpleDialogueAutoClose(wakeUpText, 2f);
                    didShowWakeUpMsg = true;
                    isRecovering = false; // 회복 모드 종료
                }

                // 이스터 에그 타이머 작동
                stayTimer += Time.deltaTime;
                CheckStayDialogues(isEN);
            }
        }
    }

    private void CheckStayDialogues(bool isEN)
    {
        // 5초 단계
        if (stayTimer >= 3f && !didShow5s)
        {
            string text = isEN ? "No time to lie down." : "누워있을 시간은 없다.";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 2f);
            didShow5s = true;
        }
        // 15초 단계
        else if (stayTimer >= 8f && !didShow15s)
        {
            string text = isEN ? "I hear the sound of graduation being delayed..." : "졸업이 밀리는 소리가 들린다...";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 2f);
            didShow15s = true;
        }
        // 30초 단계
        else if (stayTimer >= 13f && !didShow30s)
        {
            string text = isEN ? "At this rate, I'll be trapped here forever.." : "이러다가 평생 갇혀있겠어..";
            DialogueManager.Instance.ShowSimpleDialogueAutoClose(text, 3f);
            didShow30s = true;
        }
    }

    private void OnExitBed()
    {
        isPlayerInside = false;
        stayTimer = 0f;
        recoveryTimer = 0f;
        sleepDelayTimer = 0f;

        // 모든 플래그 초기화
        didShowSleepMsg = false;
        isRecovering = false;
        didShowWakeUpMsg = false;
        didShow5s = false;
        didShow15s = false;
        didShow30s = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) OnExitBed();
    }
}