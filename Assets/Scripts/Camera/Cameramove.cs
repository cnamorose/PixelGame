using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameramove : MonoBehaviour
{
    public Transform target;

    [Header("Camera Clamp Settings")]
    public bool useClamp = false;   
    public float minX, maxX;
    public float minY, maxY;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        float newX = target.position.x;
        float newY = target.position.y;

        if (useClamp)
        {

            newX = Mathf.Clamp(newX, minX, maxX);
            newY = Mathf.Clamp(newY, minY, maxY);
        }

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
