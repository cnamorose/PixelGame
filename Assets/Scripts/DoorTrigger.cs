using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoorTrigger : MonoBehaviour
{
    public TMP_Text warningText;
    private Coroutine hideRoutine;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowWarning("졸업하기 전에는 나갈 수 없다...");
        }
    }

    void ShowWarning(string msg)
    {
        warningText.text = msg;
        warningText.gameObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(1.5f);
        warningText.gameObject.SetActive(false);
    }
}
