using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Speaker", fileName = "NewSpeaker")]
public class Speaker : ScriptableObject
{
    [Header("Speaker Info")]
    public string speakerName;
    public Sprite portrait;
}