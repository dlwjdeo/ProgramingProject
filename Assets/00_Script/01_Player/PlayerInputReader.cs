using System;
using UnityEngine;

//인게임 플레이어 관련 입력만 받는 class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    //단순 행동 이벤트
    public event Action Jump;
    public event Action Interaction;
    public event Action Lamp;
    public event Action DropItem;
    public event Action Dialog;

    public bool JumpPressed {  get; private set; }
    public bool InterationPressed {  get; private set; }
    public bool LampPressed { get; private set; }
    public bool DropItemPressed { get; private set; }
    public bool DialogPressed { get; private set; }

    public bool RunPressed { get; private set; }

    private void Awake()
    {
        JumpPressed = false;
        InterationPressed = false;
        LampPressed = false;
        DropItemPressed = false;
        DialogPressed = false;
        RunPressed = false;
    }
    //입력값은 Update, 물리적 계산은 LateUpdate 
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
                break;
        }
    }

    private void HandlePlayInput()
    {
        readMove();
        detectJump();
        detectInteraction();
        detectLamp();
        detectDropItem();
        detectRun();
    }

    private void HandleDialogInput()
    {
        detectDialog();
    }

    private void readMove()
    {
        float x = 0f;
        if (KeyBindings.Hold(ActionType.MoveLeft)) x -= 1f;
        if (KeyBindings.Hold(ActionType.MoveRight)) x += 1f;
        move = new Vector2(Mathf.Clamp(x, -1f, 1f), 0f);
    }

    private void detectJump()
    {
        if(KeyBindings.Down(ActionType.Jump))
        {
            Jump?.Invoke();
            JumpPressed = true;
        }
        else
        {
            JumpPressed = false;
        }
    }

    private void detectInteraction()
    {
        if (KeyBindings.Down(ActionType.Interact)) 
        {
            Interaction?.Invoke();
            InterationPressed = true;
        }
        else
        {
            InterationPressed = false;
        }
    }

    private void detectLamp()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Lamp?.Invoke();
            LampPressed = true;
        }
        else
        {
            LampPressed = false;
        }
    }

    private void detectDropItem()
    {
        if (KeyBindings.Down(ActionType.ItemDrop))
        {
            DropItem?.Invoke();
            DropItemPressed = true;
        }
        else
        {
            DropItemPressed = false;
        }

    }

    private void detectDialog()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Z))
        {
            Dialog?.Invoke();
            DialogPressed = true;
        }
        else
        {
            DialogPressed = false;
        }
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

    //API
    public Vector2 GetMove() {  return move; }
}
