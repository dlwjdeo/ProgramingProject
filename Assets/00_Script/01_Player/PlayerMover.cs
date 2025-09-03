using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 직접적인 이동만 관리하는 class
public class PlayerMover : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float junpForce = 5f;
    [SerializeField] private int maxJump = 1;
    private int remainJunp;

    private Rigidbody2D _rigidbody2D;
    private PlayerInputReader _playerInputReader;

    private Vector2 move;
    private bool jumpRequested = false;

    //플레이어가 계단을 이용할 때 x축 이동 방지용
    private bool moveX = true;
    private bool moveY = true;


    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputReader = GetComponent<PlayerInputReader>();
        InitializeJump();
    }


    private void Update()
    {
        move = _playerInputReader.GetMove();
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
        if(jumpRequested == false) return;
        if (remainJunp <= 0) return;
        _rigidbody2D.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
        jumpRequested = false;
        remainJunp--;
    }

    //API
    public void SetMove(bool _moveX, bool _moveY)
    {
        moveX = _moveX;
        moveY = _moveY;
    }
}
