using UnityEngine;

// 적의 이동/플립 "실행"만 담당
public class EnemyMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Enemy _enemy;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public bool CanMove { get; private set; } = true;
    public float Direction { get; private set; } = 1f; // +1 right, -1 left

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    public void SetMoveEnabled(bool enabled)
    {
        CanMove = enabled;
    }

    public void MoveTowardsX(Vector3 targetWorld)
    {
        if (!CanMove) return;

        Vector3 current = transform.position;
        Vector3 target = new Vector3(targetWorld.x, current.y, current.z);

        float dirX = Mathf.Sign(target.x - current.x);
        if (Mathf.Abs(target.x - current.x) < 0.01f) dirX = 0f;

        transform.position = Vector3.MoveTowards(current, target, moveSpeed * Time.deltaTime);

        HandleFlip(dirX);
    }

    private void HandleFlip(float dirX)
    {
        if (dirX > 0.05f && Direction < 0f)
        {
            Flip();
            Direction = 1f;
        }
        else if (dirX < -0.05f && Direction > 0f)
        {
            Flip();
            Direction = -1f;
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public bool IsArrivedX(float targetX, float threshold = 0.1f)
    {
        return Mathf.Abs(transform.position.x - targetX) <= threshold;
    }

    public bool IsArrivedX(Vector3 targetWorld, float threshold = 0.1f)
    {
        return Mathf.Abs(transform.position.x - targetWorld.x) <= threshold;
    }
}
