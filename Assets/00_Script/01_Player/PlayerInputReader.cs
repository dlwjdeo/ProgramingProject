using System;
using UnityEngine;

// ????? ?¡À???? ???? ??¢¬? ??? class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    // ??? ?? ????
    public event Action Interaction;
    public event Action Lamp;
    public event Action DropItem;
    public event Action Dialog;

    public bool InterationPressed { get; private set; }
    public bool LampPressed { get; private set; }
    public bool DropItemPressed { get; private set; }
    public bool DialogPressed { get; private set; }
    public bool RunPressed { get; private set; }

    private void Awake()
    {
        InterationPressed = false;
        LampPressed = false;
        DropItemPressed = false;
        DialogPressed = false;
        RunPressed = false;
    }

    private void Update()
    {
        detectEsc();

        switch (GameManager.Instance.CurrentState)
        {
            case GameState.Playing:
                HandlePlayInput();
                break;

            case GameState.Dialog:
                HandleDialogInput();
                break;

            case GameState.Paused:
                // ??? ????
                move = Vector2.zero;
                RunPressed = false;
                break;
        }
    }

    private void HandlePlayInput()
    {
        readMove();
        detectInteraction();
        detectLamp();
        detectDropItem();
        detectRun();
    }

    private void HandleDialogInput()
    {
        // ??? ?? ??? ??? ????
        move = Vector2.zero;
        RunPressed = false;

        detectDialog();
    }

    private void readMove()
    {
        float x = 0f;
        if (KeyBindings.Hold(ActionType.MoveLeft)) x -= 1f;
        if (KeyBindings.Hold(ActionType.MoveRight)) x += 1f;
        move = new Vector2(Mathf.Clamp(x, -1f, 1f), 0f);
    }

    private void detectInteraction()
    {
        if (KeyBindings.Down(ActionType.Interact))
        {
            Interaction?.Invoke();
            InterationPressed = true;
        }
        else InterationPressed = false;
    }

    private void detectLamp()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Lamp?.Invoke();
            LampPressed = true;
        }
        else LampPressed = false;
    }

    private void detectDropItem()
    {
        if (KeyBindings.Down(ActionType.ItemDrop))
        {
            DropItem?.Invoke();
            DropItemPressed = true;
        }
        else DropItemPressed = false;
    }

    private void detectDialog()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            Dialog?.Invoke();
            DialogPressed = true;
        }
        else DialogPressed = false;
    }

    private void detectRun()
    {
        RunPressed = KeyBindings.Hold(ActionType.Run);
    }

    private void detectEsc()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SettingsUIController.Instance.Toggle();
        }
    }

    // API
    public Vector2 GetMove() => move;
}
