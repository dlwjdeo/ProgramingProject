using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�÷��̾��� �������� �̵��� �����ϴ� class
public class PlayerMover : MonoBehaviour
{
    [Header("�̵�")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJump = 1;
    private int remainJunp;

    private Rigidbody2D _rigidbody2D;
    private PlayerInputReader _playerInputReader;
    private GroundChecker _groundChecker;

    private Vector2 move;
    private bool jumpRequested = false;

    //�÷��̾ ����� �̿��� �� x�� �̵� ������
    private bool moveX = true;
    private bool moveY = true;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputReader = GetComponent<PlayerInputReader>();
        _groundChecker = GetComponentInChildren<GroundChecker>();
        InitializeJump();
    }


    private void Update()
    {
        move = _playerInputReader.GetMove();
        CheckGround();
    }

    private void FixedUpdate()
    {
        _rigidbody2D.velocity = new Vector2(move.x * playerSpeed, _rigidbody2D.velocity.y);
        TryJump();
    }

    private void OnEnable()
    {
        _playerInputReader.Jump += OnJunpPressed;
    }

    private void OnDisable()
    {
        _playerInputReader.Jump -= OnJunpPressed;
    }

    private void InitializeJump()
    {
        remainJunp = maxJump;
    }

    private void OnJunpPressed()
    {
        jumpRequested = true;
    }

    private void TryJump()
    {
        if (!_groundChecker.IsGrounded) return;
        if(!jumpRequested) return;
        if (remainJunp <= 0) return;
        _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpRequested = false;
        remainJunp--;
    }

    private void CheckGround()
    {
        if(_groundChecker.IsGrounded) InitializeJump();
    }

    //API
    public void SetMove(bool _moveX, bool _moveY)
    {
        moveX = _moveX;
        moveY = _moveY;
    }
}
