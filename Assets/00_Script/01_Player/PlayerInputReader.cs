using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//인게임 플레이어 관련 인풋만 받는 class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    public event Action Jump;
    public event Action Interaction;

    private void Update()
    {
        ReadMove();
        DetectJump();
        DetectInteraction();
    }

    private void ReadMove()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void DetectJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump?.Invoke();
        }
    }

    private void DetectInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            Interaction?.Invoke();
        }
    }

    //API
    public Vector2 GetMove() {  return move; }
}
