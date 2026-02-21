using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Rigidbody2D rb;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    public float CurrentSpeedX => rb != null ? Mathf.Abs(rb.velocity.x) : 0f;
    public bool IsMovingX { get; private set; }

    public bool CanMove { get; private set; } = true;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void SetMoveEnabled(bool enabled)
    {
        CanMove = enabled;
        if (!enabled) Stop();
    }

    public void Stop()
    {
        IsMovingX = false;
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    public float MoveTowardsX(Vector3 targetWorld, float arriveThreshold = 0.05f)
    {
        if (!CanMove)
        {
            Stop();
            return 0f;
        }

        float currentX = rb.position.x;
        float targetX = targetWorld.x;

        float deltaX = targetX - currentX;

        if (Mathf.Abs(deltaX) <= arriveThreshold)
        {
            Stop();
            return 0f;
        }

        float dirX = Mathf.Sign(deltaX);
        IsMovingX = true;
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        return dirX;
    }

    public bool IsArrivedX(Vector3 targetWorld, float threshold = 0.1f)
        => Mathf.Abs(rb.position.x - targetWorld.x) <= threshold;
}
