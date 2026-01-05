using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTestCaller : MonoBehaviour
{
    GroundSpikeManager groundSpike;
    SideTridentAttack sideTrident;
    BouncingBallAttack bouncingBall;

    void Start()
    {
        // 바닥 가시 매니저
        groundSpike = GroundSpikeManager.Instance;
        if (groundSpike == null)
            Debug.LogError("GroundSpikeManager 없음!");

        // 양옆 삼지창
        sideTrident = GetComponent<SideTridentAttack>();
        if (sideTrident == null)
            Debug.LogError("SideTridentAttack 컴포넌트 없음!");

        // 공 튀기기 공격
        bouncingBall = GetComponent<BouncingBallAttack>();
        if (bouncingBall == null)
            Debug.LogError("BouncingBallAttack 컴포넌트 없음!");
    }

    void Update()
    {
        // 바닥 가시
        if (Input.GetKeyDown(KeyCode.G) && groundSpike != null)
        {
            groundSpike.StartEvenOddWave();
        }

        // 양옆 삼지창
        if (Input.GetKeyDown(KeyCode.T) && sideTrident != null)
        {
            sideTrident.StartSideAttack();
        }

        // 공 튀기기
        if (Input.GetKeyDown(KeyCode.B) && bouncingBall != null)
        {
            bouncingBall.StartBouncingAttack();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GetComponent<SkyTridentAttack>().StartSkyAttack();
        }
    }
}