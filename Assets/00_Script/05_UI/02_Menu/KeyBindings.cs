using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/Key Bindings (Defaults)", fileName = "KeyBindings_Defaults")]
public class KeyBindings : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public ActionType action;
        public KeyCode key;
    }

    public List<Entry> defaults = new List<Entry>
    {
        new Entry { action = ActionType.MoveLeft,  key = KeyCode.A },
        new Entry { action = ActionType.MoveRight, key = KeyCode.D },
        new Entry { action = ActionType.Jump,      key = KeyCode.Space },
        new Entry { action = ActionType.Interact,  key = KeyCode.E },
        new Entry { action = ActionType.Pause,     key = KeyCode.Escape },
    };
}