using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RebindButton : MonoBehaviour
{
    [SerializeField] private ActionType action;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI actionName;
    [SerializeField] private TextMeshProUGUI statusText; 

    private bool waiting;
    private void Start()
    {
        SetName();
    }
    private void OnEnable()
    {
        if (button != null) button.onClick.AddListener(Begin);
        Refresh();
    }

    private void OnDisable()
    {
        if (button != null) button.onClick.RemoveListener(Begin);
        waiting = false;
    }

    private void Update()
    {
        if (!waiting) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            waiting = false;
            SetStatus("");
            Refresh();
            return;
        }

        if (!Input.anyKeyDown) return;

        KeyCode pressed = DetectPressedKey();
        if (pressed == KeyCode.None) return;

        // 마우스 버튼 제외(원하면)
        if (IsMouseKey(pressed)) return;

        KeyBindings.Set(action, pressed);
        waiting = false;
        SetStatus("");
        Refresh();
    }

    private void Begin()
    {
        if (waiting) return;
        waiting = true;
        SetStatus("Press a key... (ESC to cancel)");
    }

    private void Refresh()
    {
        if (keyText != null)
            keyText.text = KeyBindings.Get(action).ToString();
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    private void SetName()
    {
        actionName.text = action.ToString();
    }
    private static KeyCode DetectPressedKey()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKeyDown(key)) return key;

        return KeyCode.None;
    }

    private static bool IsMouseKey(KeyCode key)
    {
        return key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6;
    }
}