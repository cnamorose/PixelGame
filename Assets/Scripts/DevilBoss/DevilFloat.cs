using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevilFloat : MonoBehaviour
{
    public float floatAmplitude = 0.2f;
    public float floatSpeed = 2f;

    Vector3 basePosition;

    void OnEnable()
    {
        basePosition = transform.position;
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = basePosition + Vector3.up * offsetY;
    }

    public void ResetBasePosition()
    {
        basePosition = transform.position;
    }
}