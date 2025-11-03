using UnityEngine;

[System.Serializable]
public class DialogLine
{
    public Speaker speaker;
    [TextArea(2, 5)] public string text;
}