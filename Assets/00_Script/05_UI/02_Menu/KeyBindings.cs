using System;
using System.Collections.Generic;
using UnityEngine;

public static class KeyBindings
{
    private static readonly Dictionary<ActionType, KeyCode> defaults = new()
    {
        { ActionType.MoveLeft,  KeyCode.A },
        { ActionType.MoveRight, KeyCode.D },
        { ActionType.Interact,  KeyCode.E },
        { ActionType.Run,       KeyCode.LeftShift},
        { ActionType.ItemDrop,  KeyCode.Q },
        { ActionType.Pause,     KeyCode.Escape },
    };

    private const string Prefix = "TSUKINO_KEY_";

    public static KeyCode Get(ActionType action)
    {
        string k = Prefix + action;
        string saved = PlayerPrefs.GetString(k, string.Empty);

        if (!string.IsNullOrEmpty(saved) && Enum.TryParse(saved, out KeyCode key))
            return key;

        return defaults.TryGetValue(action, out var def) ? def : KeyCode.None;
    }

    public static void Set(ActionType action, KeyCode key)
    {
        PlayerPrefs.SetString(Prefix + action, key.ToString());
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        foreach (var a in Enum.GetValues(typeof(ActionType)))
            PlayerPrefs.DeleteKey(Prefix + (ActionType)a);

        PlayerPrefs.Save();
    }

    // ���� �Լ�
    public static bool Down(ActionType action) => Input.GetKeyDown(Get(action));
    public static bool Hold(ActionType action) => Input.GetKey(Get(action));
}