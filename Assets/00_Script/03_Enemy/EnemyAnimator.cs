using UnityEngine;

public sealed class EnemyAnimator : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform visualRoot;

    [Header("Params")]
    [SerializeField] private string locomotionParam = "Locomotion";

    private Enemy _enemy;
    private int _locomotionHash;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (visualRoot == null) visualRoot = transform;

        _locomotionHash = Animator.StringToHash(locomotionParam);
    }

    private void Update()
    {
        if (_enemy == null || animator == null) return;

        bool isMoving = false;
        if (_enemy.Mover != null)
        {
            isMoving = _enemy.Mover.IsMovingX || _enemy.Mover.CurrentSpeedX > 0.01f;
        }

        int locomotion = 0;
        if (isMoving)
        {
            locomotion = _enemy.MoveMode == EnemyMoveMode.Run ? 2 : 1;
        }

        animator.SetInteger(_locomotionHash, locomotion);

        float dir = _enemy.Direction;
        if (Mathf.Abs(dir) > 0.01f)
        {
            Vector3 s = visualRoot.localScale;
            s.x = Mathf.Abs(s.x) * (dir > 0f ? 1f : -1f);
            visualRoot.localScale = s;
        }
    }
}