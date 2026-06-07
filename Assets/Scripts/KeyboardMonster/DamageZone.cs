using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    [Header("리스폰 포인트 목록 (여러 개 등록 가능)")]
    public Transform[] respawnPoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1) 생명 감소
            PlayerLifeManager.Instance.LoseLife();

            // 2) 생명이 남아 있다면 리스폰
            if (PlayerLifeManager.Instance.currentLife > 0)
            {
                // 가장 가까운 리스폰 포인트 찾기
                Transform closestPoint = GetClosestRespawnPoint(other.transform.position);

                if (closestPoint != null)
                {
                    other.transform.position = closestPoint.position;
                }
                else
                {
                    Debug.LogWarning("등록된 리스폰 포인트가 없습니다!");
                }
            }
        }
    }

    // 플레이어 위치 기준으로 가장 가까운 포인트를 계산하는 함수
    private Transform GetClosestRespawnPoint(Vector3 playerPosition)
    {
        if (respawnPoints == null || respawnPoints.Length == 0) return null;

        Transform closest = null;
        float shortestDistance = Mathf.Infinity; // 비교를 위해 처음엔 무한대 값으로 설정

        // 등록된 모든 리스폰 포인트를 하나씩 검사
        foreach (Transform point in respawnPoints)
        {
            if (point == null) continue;

            // 플레이어와 리스폰 포인트 사이의 거리 계산
            float distance = Vector3.Distance(playerPosition, point.position);

            // 방금 계산한 거리가 기존에 알던 최소 거리보다 더 짧다면 갱신
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = point;
            }
        }

        return closest;
    }
}