using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float walkMoveSpeed = 2f;
    [SerializeField] private float runMoveSpeed = 3f;

    public EnemyVision EnemyVision { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }

    [SerializeField] private EnemyStateType state;
    public EnemyStateType State => state;

    public RoomController CurrentRoom { get; private set; }

    public EnemyMover Mover { get; private set; }
    public float DefaultMoveSpeed => walkMoveSpeed;
    public float WalkMoveSpeed => walkMoveSpeed;
    public float RunMoveSpeed => runMoveSpeed;
    public EnemyMoveMode MoveMode { get; private set; } = EnemyMoveMode.Walk;

    public float Direction { get; private set; } = 1f;
    public ChaseTarget CurrentChaseTarget { get; private set; }

    private void Awake()
    {
        Mover = GetComponent<EnemyMover>();
        SetMoveMode(EnemyMoveMode.Walk);

        StateMachine = new EnemyStateMachine(this);
        EnemyVision = GetComponentInChildren<EnemyVision>();
    }

    private void OnEnable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedEnemyRoom += SetCurrentRoom;

        if (RoomManager.Instance != null && RoomManager.Instance.EnemyRoom != null)
            SetCurrentRoom(RoomManager.Instance.EnemyRoom);
    }

    private void OnDisable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnChangedEnemyRoom -= SetCurrentRoom;
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }
    public void SetChaseTargetPlayer(Transform playerTf, RoomController playerRoom)
    {
        CurrentChaseTarget = ChaseTarget.FromPlayer(playerTf, playerRoom);
    }

    public void SetChaseTargetRoom(RoomController room)
    {
        CurrentChaseTarget = ChaseTarget.FromRoom(room);
    }

    public void ClearChaseTarget()
    {
        CurrentChaseTarget = default;
    }
    public void SetStateType(EnemyStateType type) => state = type;

    public void SetMove(bool move)
    {
        if (Mover != null) Mover.SetMoveEnabled(move);
    }

    public void SetMoveSpeed(float speed)
    {
        if (Mover != null) Mover.MoveSpeed = speed;
    }

    public void SetMoveMode(EnemyMoveMode mode)
    {
        MoveMode = mode;

        float targetSpeed = mode == EnemyMoveMode.Run ? runMoveSpeed : walkMoveSpeed;
        if (Mover != null) Mover.MoveSpeed = targetSpeed;
    }

    public void SetCurrentRoom(RoomController room)
    {
        CurrentRoom = room;
    }

    public void SetDirection(float dir)
    {
        dir = Mathf.Sign(dir);
        if (dir == 0f) return;

        if (Direction != dir)
        {
            Direction = dir;
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

}

public enum EnemyMoveMode
{
    Walk,
    Run,
}

public enum ChaseTargetType
{
    None,
    Player,
    Room,
}

public struct ChaseTarget
{
    public ChaseTargetType Type;
    public Transform Transform;      // Player�� player.transform, Room�̸� room.transform(�Ǵ� center anchor)
    public RoomController Room;      // Player�� player.CurrentRoom, Room�̸� �ڱ� �ڽ�

    public bool IsValid => Type != ChaseTargetType.None && Transform != null && Room != null;

    public static ChaseTarget FromPlayer(Transform playerTf, RoomController playerRoom)
        => new ChaseTarget { Type = ChaseTargetType.Player, Transform = playerTf, Room = playerRoom };

    public static ChaseTarget FromRoom(RoomController room)
        => new ChaseTarget { Type = ChaseTargetType.Room, Transform = room.transform, Room = room };
}