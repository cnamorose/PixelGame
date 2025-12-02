using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Sequence")]
public class DialogueSequence : ScriptableObject
{
    public string id;  // 이벤트 ID (예: devil_intro_1)
    public List<DialogueLine> lines;
}

