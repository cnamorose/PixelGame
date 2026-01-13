using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    Vector3 origin;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            origin = transform.localPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Shake(float time, float power)
    {
        StartCoroutine(ShakeRoutine(time, power));
    }

    IEnumerator ShakeRoutine(float time, float power)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.localPosition = origin +
                (Vector3)Random.insideUnitCircle * power;
            yield return null;
        }
        transform.localPosition = origin;
    }
}
