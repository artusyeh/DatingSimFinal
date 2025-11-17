using System;
using UnityEngine;

[Serializable]
public class Choice
{
    public string text;
    public string next;   // id of the next node
}

[Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    [TextArea] public string text;
    public Choice[] choices;  // null or empty if this is a linear node
    public string next;       // used if there are NO choices
}

[Serializable]
public class DialogueRoot
{
    public string startNode;
    public DialogueNode[] nodes;
}
