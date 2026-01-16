using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSceneManager : MonoBehaviour
{
    [Header("Cutscene Runners")]
    public GameObject boyRunner;
    public GameObject girlRunner;

    public Camera runCamera;

    void Start()
    {
        Debug.Log("RunSceneManager Start");

        // 캐릭터 선택 정보 가져오기
        string selected =
            PlayerPrefs.GetString("SelectedCharacter", "Boy");

        Debug.Log("SelectedCharacter = " + selected);

        bool isBoy = selected == "Boy";

        // 하나만 활성화
        boyRunner.SetActive(isBoy);
        girlRunner.SetActive(!isBoy);

        // 활성화된 러너 가져오기
        GameObject activeRunner = isBoy ? boyRunner : girlRunner;

        // 카메라 타겟 연결
        if (runCamera != null)
        {
            RunCameraFollow camFollow =
                runCamera.GetComponent<RunCameraFollow>();

            if (camFollow != null)
            {
                camFollow.target = activeRunner.transform;
            }
            else
            {
                Debug.LogWarning("RunCameraFollow not found on runCamera");
            }
        }
    }
}