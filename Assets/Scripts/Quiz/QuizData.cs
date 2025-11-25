using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuiz", menuName = "Quiz/QuizData")]
public class QuizData : ScriptableObject
{
    [TextArea]
    public string question;

    public string[] answers = new string[4];

    public int correctIndex;
}
