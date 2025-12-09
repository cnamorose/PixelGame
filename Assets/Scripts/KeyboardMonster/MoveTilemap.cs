using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTilemap : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    private Vector3 target;

    void Start()
    {
        target = pointB;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            target = (target == pointA) ? pointB : pointA;
        }
    }
}
