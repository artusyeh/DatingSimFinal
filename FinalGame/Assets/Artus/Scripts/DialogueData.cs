using System;
using UnityEngine;

[Serializable]
public class Choice
{
    public string text;
    public string next;
    public bool isCorrect;
    public float delay = 0f;
}

[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    [TextArea] public string text;

    public Choice[] choices;
    public string next;

    public string expression;
    public bool forceTimerEnd;

    public string shake; // "good", "bad"

    // Jumpscare support
    public bool jumpscare;
    public string jumpscareNext;

    // Zoom support
    public string zoomType;     // "in", "out", "pan"
    public float zoomTarget;    // MUST be float
    public float zoomDuration;  
    public float zoomHold;

    public bool blackout;

    public bool creepyAikoJumpscare;   

    public string endType; // "good", "bad"

    public bool stopTimerAndHeartFX;
}

[Serializable]
public class DialogueRoot
{
    public string startNode;
    public DialogueNode[] nodes;
}
