using UnityEngine;

//플레이어의 직접적인 이동만 관리하는 class
public class PlayerMover : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJump = 1;
    private int remainJump;

    private bool canMove = true;

    public Rigidbody2D Rigidbody { get; private set; }
    public GroundChecker GroundChecker { get; private set; }
    private PlayerInputReader playerInputReader;

    public Vector2 Move { get; private set; }
    public bool CanJump => GroundChecker.IsGrounded && remainJump > 0;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        playerInputReader = GetComponent<PlayerInputReader>();
        GroundChecker = GetComponentInChildren<GroundChecker>();
        ResetJump();
    }

    private void OnEnable()
    {
        playerInputReader.Jump += OnJumpPressed;
    }

    private void OnDisable()
    {
        playerInputReader.Jump -= OnJumpPressed;
    }

    private void Update()
    {
        Move = playerInputReader.GetMove();
        if (GroundChecker.IsGrounded) ResetJump();
    }

    private void FixedUpdate()
    {
        if (!canMove)
        {
            Rigidbody.velocity = new Vector2(0, Rigidbody.velocity.y);
            return;
        }

        Rigidbody.velocity = new Vector2(Move.x * playerSpeed, Rigidbody.velocity.y);
    }

    private void OnJumpPressed()
    {
        if (CanJump)
            Player.Instance.PlayerStateMachine.ChangeState(Player.Instance.PlayerStateMachine.Jump);
    }

    private void ResetJump() => remainJump = maxJump;

    // API
    public void DoJump()
    {
        Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        remainJump--;
    }

    public void SetMove(bool move) => canMove = move;
}