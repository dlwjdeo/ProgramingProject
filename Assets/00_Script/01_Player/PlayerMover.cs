using UnityEngine;

// �÷��̾��� ���� �̵�/���� "����"�� ���
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMover : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float baseSpeed = 5f;

    private Rigidbody2D _rb;
    private GroundChecker _ground;

    private float _speedMultiplier = 1f;
    private bool _canMove = true;

    public Rigidbody2D Rigidbody => _rb;
    public GroundChecker GroundChecker => _ground;

    public Vector2 MoveInput { get; private set; }
    public bool IsGrounded => _ground != null && _ground.IsGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ground = GetComponentInChildren<GroundChecker>();
    }

    private void Update()
    {
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
}
