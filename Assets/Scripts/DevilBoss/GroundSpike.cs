using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpike : MonoBehaviour
{
    [Header("Spike Motion")]
    public float riseHeight = 1.0f;
    public float riseTime = 0.2f;
    public float stayTime = 0.8f;
    public float downTime = 0.2f;

    Vector3 startPos;
    Vector3 endPos;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + Vector3.up * riseHeight;

        transform.position = startPos;
        StartCoroutine(SpikeRoutine());
    }

    IEnumerator SpikeRoutine()
    {
        // 올라오기
        yield return Move(startPos, endPos, riseTime);

        // 유지
        yield return new WaitForSeconds(stayTime);

        // 내려가기
        yield return Move(endPos, startPos, downTime);

        Destroy(gameObject);
    }

    IEnumerator Move(Vector3 from, Vector3 to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, t / time);
            yield return null;
        }
        transform.position = to;
    }
}