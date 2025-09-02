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

    //�÷��̾ ����� �̿��� �� x�� �̵� ������
    private bool moveX = true;
    private bool moveY = true;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputReader = GetComponent<PlayerInputReader>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector2 move = _playerInputReader.GetMove();
        _rigidbody2D.velocity = new Vector2(move.x * playerSpeed, _rigidbody2D.velocity.y);
    }
}
