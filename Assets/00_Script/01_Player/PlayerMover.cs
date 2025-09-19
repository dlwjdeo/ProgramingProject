using UnityEngine;

//플레이어의 직접적인 이동만 관리하는 class
public class PlayerMover : MonoBehaviour
{
    [Header("이동")]
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private int maxJump = 1;
    private int remainJunp;

    private bool canMove = true;

    public Rigidbody2D _rigidbody2D { get; private set; }
    private PlayerInputReader playerInputReader;
    public GroundChecker GroundChecker { get; private set; }

    public Vector2 Move { get; private set; }

    public PlayerStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        playerInputReader = GetComponent<PlayerInputReader>();
        GroundChecker = GetComponentInChildren<GroundChecker>();
        initializeJump();

        StateMachine = new PlayerStateMachine();
    }

    private void OnEnable()
    {
        playerInputReader.Jump += onJunpPressed;
    }

    private void OnDisable()
    {
        playerInputReader.Jump -= onJunpPressed;
    }

    private void Start()
    {
        StateMachine.ChangeState(StateMachine.Idle, this);
    }

    private void Update()
    {
        Move = playerInputReader.GetMove();
        checkGround();

        StateMachine.Update(this);
    }

    private void FixedUpdate()
    {
        if (!canMove) //입력이 남아있을 때 미끄러짐 방지
        {
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            return;
        }

        _rigidbody2D.velocity = new Vector2(Move.x * playerSpeed, _rigidbody2D.velocity.y);
    }

    private void initializeJump()
    {
        remainJunp = maxJump;
    }

    private void onJunpPressed()
    {
        if (GroundChecker.IsGrounded && remainJunp > 0)
        {
            StateMachine.ChangeState(StateMachine.Jump, this);
        }
    }

    private void checkGround()
    {
        if (GroundChecker.IsGrounded) initializeJump();
    }
    //API
    public void DoJump()
    {
        _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        remainJunp--;
    }

    public void SetMove(bool move)
    {
        canMove = move;
    }
}
