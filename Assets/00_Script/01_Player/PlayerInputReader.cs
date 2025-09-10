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

    //입력값은 Update, 물리적 계산은 LateUpdate 
    private void Update()
    {
        readMove();
        detectJump();
        detectInteraction();
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
        }
    }

    private void detectInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Interaction?.Invoke();
        }
    }

    private void detectLamp()
    {
        if (!Input.GetKeyDown(KeyCode.F))
        {
            Lamp?.Invoke();
        }
    }

    //API
    public Vector2 GetMove() {  return move; }
}
