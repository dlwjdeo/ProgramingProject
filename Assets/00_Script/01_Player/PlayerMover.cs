using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//플레이어의 직접적인 이동만 관리하는 class
public class PlayerMover : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJump = 1;
    private int remainJunp;

    private Rigidbody2D rigidbody2D;
    private PlayerInputReader playerInputReader;
    private GroundChecker groundChecker;

    private Vector2 move;
    private bool jumpRequested = false;

    //플레이어가 계단을 이용할 때 x축 이동 방지용
    private bool moveX = true;
    private bool moveY = true;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerInputReader = GetComponent<PlayerInputReader>();
        groundChecker = GetComponentInChildren<GroundChecker>();
        initializeJump();
    }


    private void Update()
    {
        move = playerInputReader.GetMove();
        checkGround();
    }

    private void FixedUpdate()
    {
        rigidbody2D.velocity = new Vector2(move.x * playerSpeed, rigidbody2D.velocity.y);
        tryJump();
    }

    private void OnEnable()
    {
        playerInputReader.Jump += onJunpPressed;
    }

    private void OnDisable()
    {
        playerInputReader.Jump -= onJunpPressed;
    }

    private void initializeJump()
    {
        remainJunp = maxJump;
    }

    private void onJunpPressed()
    {
        jumpRequested = true;
    }

    private void tryJump()
    {
        if (!groundChecker.IsGrounded) return;
        if(!jumpRequested) return;
        if (remainJunp <= 0) return;
        rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpRequested = false;
        remainJunp--;
    }

    private void checkGround()
    {
        if(groundChecker.IsGrounded) InitializeJump();
    }

    //API
    public void SetMove(bool _moveX, bool _moveY)
    {
        moveX = _moveX;
        moveY = _moveY;
    }
}
