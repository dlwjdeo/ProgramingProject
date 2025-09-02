using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾��� �������� �̵��� �����ϴ� class
public class PlayerMover : MonoBehaviour
{
    [Header("�̵�")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float junpForce = 5f;

    private Rigidbody2D _rigidbody2D;
    private PlayerInputReader _playerInputReader;

    private Vector2 move;

    //�÷��̾ ����� �̿��� �� x�� �̵� ������
    private bool moveX = true;
    private bool moveY = true;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        move = _playerInputReader.GetMove();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(move.x * playerSpeed, _rigidbody2D.velocity.y);
    }

    //API
    public void SetMove(bool _moveX, bool _moveY)
    {
        moveX = _moveX;
        moveY = _moveY;
    }
}
