using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageButton_L : MonoBehaviour
{
    public void OnClick_Korean()
    {
        GameManager_L.Instance.SetLanguage(Language.KR);
    }

    public void OnClick_English()
    {
        GameManager_L.Instance.SetLanguage(Language.EN);
    }
}
