using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float viewRange = 5f;

    public EnemyVision EnemyVision {  get; private set; }

    public float DefultMoveSpeed { get; private set; } = 2f;

    public RoomController LastKnownRoom {  get; private set; }

    public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private EnemyStateType state;

    public bool CanMove { get; private set; }

    [SerializeField] private BoxCollider2D _collider2D;
    [SerializeField] private BoxCollider2D detectCollier;

    public RoomController CurrentRoom;// { get; private set; }


    public float LastRoomEnterTime { get; private set; }

    public float Direction { get; private set; } = 1;

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this);
        EnemyVision = GetComponentInChildren<EnemyVision>();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }
    private void OnEnable()
    {
        RoomManager.Instance.OnChangedEnemyRoom += SetCurrentRoom;
    }
    private void OnDisable()
    {
        RoomManager.Instance.OnChangedEnemyRoom -= SetCurrentRoom;
    }
    public void SetStateType(EnemyStateType type) { state = type; }

    public void MoveToRoom(RoomController targetRoom)
    {
        if (targetRoom == null || CurrentRoom == null) return;

        // 같은 층
        if (targetRoom.Floor == CurrentRoom.Floor)
        {
            Vector3 targetPoint = targetRoom.Collider2D.bounds.center;
            MoveTowards(targetPoint);
        }
        else
        {
            MoveToPortal(targetRoom);
        }
    }

    public void MoveToPortal(RoomController targetRoom)
    {
        Portal portal = RoomManager.Instance.FindClosestPortal(CurrentRoom.Floor, targetRoom.Floor, transform.position);

        if (portal == null) return;

        MoveTowards(portal.transform.position);

        if (IsArrived(portal.transform.position))
            portal.InteractPortal(transform, false);
    }

    public void MoveTowards(Vector3 target)
    {
        Vector3 current = transform.position;
        Vector3 targetPos = new Vector3(target.x, current.y, current.z);
        Vector3 dir = (targetPos - current).normalized;

        transform.position = Vector3.MoveTowards(current, targetPos, moveSpeed * Time.deltaTime);

        HandleFlip(dir.x);
    }
    private void HandleFlip(float dirX)
    {
        if (dirX > 0.05f && Direction < 0)
        {
            Flip();
            Direction = 1;
        }
        else if (dirX < -0.05f && Direction > 0)
        {
            Flip();
            Direction = -1;
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    public void SetMove(bool move) => CanMove = move; //TODO: 실제 멈추는 로직 필요
    private void setLastRoomEnterTime() => LastRoomEnterTime = Time.time;
    public bool IsArrived(RoomController targetRoom)
    {
        float targetX = targetRoom.Collider2D.bounds.center.x;
        float distanceX = Mathf.Abs(transform.position.x - targetX);
        return distanceX <= 0.1f;
    }

    public bool IsArrived(Vector3 pos)
    {
        float targetX = pos.x;
        float distanceX = Mathf.Abs(transform.position.x - targetX);
        return distanceX <= 0.1f;
    }

    public void SetLastKnownRoom(RoomController playerRoom) { LastKnownRoom = playerRoom; }

    public void SetMoveSpeed(float speed) { moveSpeed = speed; }
    public void SetCurrentRoom(RoomController room) { CurrentRoom = room; }
}