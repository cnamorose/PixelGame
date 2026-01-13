using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorExitTrigger : MonoBehaviour
{
    public ElevatorArrival elevator;
    bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        elevator.OnPlayerExit();
    }
}