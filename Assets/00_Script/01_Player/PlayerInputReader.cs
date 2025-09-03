using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ΰ��� �÷��̾� ���� ��ǲ�� �޴� class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    public event Action Jump;

    private void Update()
    {
        ReadMove();
        DetectJump();
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


    //API
    public Vector2 GetMove() {  return move; }
}
