using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ΰ��� �÷��̾� ���� ��ǲ�� �޴� class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    private void Update()
    {
        ReadMove();
    }

    private void ReadMove()
    {
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }


    //API
    public Vector2 GetMove() {  return move; }
}
