using UnityEngine;

// 플레이어의 물리 이동/점프 "실행"만 담당
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Jump")]
    [SerializeField] private int maxJump = 1;

    private Rigidbody2D _rb;
    private GroundChecker _ground;

    private float _speedMultiplier = 1f;
    private bool _canMove = true;
    private int _remainJump;

    public Rigidbody2D Rigidbody => _rb;
    public GroundChecker GroundChecker => _ground;

    public Vector2 MoveInput { get; private set; }
    public bool IsGrounded => _ground != null && _ground.IsGrounded;

    public bool CanJump => _remainJump > 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponentInChildren<GroundChecker>();
        ResetJump();
    }

    private void Update()
    {
        if (IsGrounded) ResetJump();
    }

    private void FixedUpdate()
    {
        if (!_canMove)
        {
            _rb.velocity = new Vector2(0f, _rb.velocity.y);
            return;
        }

        _rb.velocity = new Vector2(MoveInput.x * baseSpeed * _speedMultiplier, _rb.velocity.y);
    }

    private void ResetJump() => _remainJump = maxJump;

    public void SetMoveInput(Vector2 input)
    {
        MoveInput = input;
    }

    public void SetMoveEnabled(bool enabled)
    {
        _canMove = enabled;
        if (!enabled) MoveInput = Vector2.zero;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = multiplier;
    }

    public void DoJump()
    {
        if (!CanJump) return;

        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        _remainJump--;
    }
}
