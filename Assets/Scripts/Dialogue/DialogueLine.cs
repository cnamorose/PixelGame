using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea]
    public string text;

    public bool requiresName;
    public bool shakeCamera;
}

