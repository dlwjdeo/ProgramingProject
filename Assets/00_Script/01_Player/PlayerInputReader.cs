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
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void detectJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
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
        if (Input.GetKeyDown(KeyCode.E)) 
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
        if (Input.GetKeyDown(KeyCode.Q))
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
        RunPressed = Input.GetKey(KeyCode.LeftShift);
    }

    //API
    public Vector2 GetMove() {  return move; }
}
