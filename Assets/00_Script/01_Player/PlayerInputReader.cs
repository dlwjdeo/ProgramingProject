using System;
using UnityEngine;

//�ΰ��� �÷��̾� ���� �Է¸� �޴� class
public class PlayerInputReader : MonoBehaviour
{
    private Vector2 move;

    //�ܼ� �ൿ �̺�Ʈ
    public event Action Jump;
    public event Action Interaction;

    //�Է°��� Update, ������ ����� LateUpdate 
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
