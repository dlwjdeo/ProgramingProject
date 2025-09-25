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
    
    public bool JumpPressed {  get; private set; }
    public bool InterationPressed {  get; private set; }
    public bool LampPressed { get; private set; }

    private void Awake()
    {
        JumpPressed = false;
        InterationPressed = false;
        LampPressed = false;
    }
    //입력값은 Update, 물리적 계산은 LateUpdate 
    private void Update()
    {
        readMove();
        detectJump();
        detectInteraction();
        detectLamp();
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

    //API
    public Vector2 GetMove() {  return move; }
}
