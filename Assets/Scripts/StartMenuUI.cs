using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuUI : MonoBehaviour
{
    public GameObject howToPlayUI;

    public void Show()
    {
        howToPlayUI.SetActive(true);
    }

    public void Hide()
    {
        howToPlayUI.SetActive(false);
    }
}
