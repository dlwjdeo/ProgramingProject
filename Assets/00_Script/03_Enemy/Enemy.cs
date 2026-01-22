using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float defaultMoveSpeed = 2f;

    public EnemyVision EnemyVision { get; private set; }
    public EnemyStateMachine StateMachine { get; private set; }

    [SerializeField] private EnemyStateType state;
    public EnemyStateType State => state;

    public RoomController CurrentRoom { get; private set; }

    public EnemyMover Mover { get; private set; }
    public float DefaultMoveSpeed => defaultMoveSpeed;

    public float Direction { get; private set; } = 1f;

    public Transform Target { get; private set; }
    public RoomController TargetRoom { get; private set; }
    public ChaseTarget CurrentChaseTarget { get; private set; }

    private void Awake()
    {
        Mover = GetComponent<EnemyMover>();
        if (Mover != null) Mover.MoveSpeed = defaultMoveSpeed;

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

    public void SetCurrentRoom(RoomController room)
    {
        CurrentRoom = room;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
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

    public void ChaseCurrentTargetWithPortal(float arriveThreshold = 0.05f)
    {
        if (Mover == null || CurrentRoom == null) return;

        var t = CurrentChaseTarget;
        if (!t.IsValid) return;

        // 같은 층이면 "그 대상"으로 직행
        if (t.Room.Floor == CurrentRoom.Floor)
        {
            float dir = Mover.MoveTowardsX(t.Transform.position, arriveThreshold);
            SetDirection(dir);
            return;
        }

        // 다른 층이면 포탈을 임시 목표로
        Portal portal = RoomManager.Instance != null
            ? RoomManager.Instance.FindClosestPortal(CurrentRoom.Floor, t.Room.Floor, transform.position)
            : null;

        if (portal == null) return;

        float pdir = Mover.MoveTowardsX(portal.transform.position, arriveThreshold);
        SetDirection(pdir);

        if (Mover.IsArrivedX(portal.transform.position))
            portal.InteractPortal(transform, false);
    }
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
    public Transform Transform;      // Player면 player.transform, Room이면 room.transform(또는 center anchor)
    public RoomController Room;      // Player면 player.CurrentRoom, Room이면 자기 자신

    public bool IsValid => Type != ChaseTargetType.None && Transform != null && Room != null;

    public static ChaseTarget FromPlayer(Transform playerTf, RoomController playerRoom)
        => new ChaseTarget { Type = ChaseTargetType.Player, Transform = playerTf, Room = playerRoom };

    public static ChaseTarget FromRoom(RoomController room)
        => new ChaseTarget { Type = ChaseTargetType.Room, Transform = room.transform, Room = room };
}