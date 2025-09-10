using System;
using UnityEngine;

//�ΰ��� �÷��̾� ���� �Է¸� �޴� class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    //�ܼ� �ൿ �̺�Ʈ
    public event Action Jump;
    public event Action Interaction;
    public event Action Lamp;

    //�Է°��� Update, ������ ����� LateUpdate 
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
