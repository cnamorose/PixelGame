using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorExitTrigger : MonoBehaviour
{
    public ElevatorArrival elevator;
    bool triggered = false;

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        elevator.OnPlayerExit();
    }
}
