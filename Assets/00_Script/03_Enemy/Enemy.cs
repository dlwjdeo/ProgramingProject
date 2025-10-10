using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float viewRange = 5f;

    public float DefultMoveSpeed { get; private set; } = 2f;

    public float ViewRange => viewRange;
    public RoomController LastKnownRoom {  get; private set; }

    public bool CanSeePlayer { get; private set; }

    public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private EnemyStateType state;

    public bool CanMove { get; private set; }

    [SerializeField] private BoxCollider2D _collider2D;
    [SerializeField] private BoxCollider2D detectCollier;

    public RoomController CurrentRoom { get; private set; }


    public float LastRoomEnterTime { get; private set; }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this);
    }

    private void Update()
    {
        updateCurrentRoom();
        updateVision();
        StateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }

    private void updateCurrentRoom()
    {
        Vector2 center = _collider2D.bounds.center;

        foreach (var room in RoomManager.Instance.Rooms)
        {
            if (room == null) continue;

            if (room.Collider2D.OverlapPoint(center))
            {
                if (room != CurrentRoom)
                {
                    CurrentRoom = room;
                    setLastRoomEnterTime();
                }
                return;
            }
        }
    }

    private void updateVision()
    {
        var player = Player.Instance;
        var playerRoom = Player.Instance.CurrentRoom;
        if (playerRoom == null || playerRoom != CurrentRoom)
        {
            CanSeePlayer = false;
            return;
        }

        //TODO: 적이 바라보는 방향만 가능하게 수정
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < viewRange)
        {
            CanSeePlayer = true;
            return;
        }
    }
    public void SetStateType(EnemyStateType type) { state = type; }
    public void MoveAI(RoomController targetRoom, bool chasePlayer = false)
    {
        if (targetRoom == null || CurrentRoom == null)
            return;

        if (targetRoom.Floor == CurrentRoom.Floor)
        {
            if (chasePlayer && Player.Instance != null) //플레이어 추적
            {
                Vector3 playerPos = Player.Instance.transform.position;
                moveToTarget(playerPos);
            }
            else //방 중심 추적
            {
                Vector3 targetPoint = targetRoom.Collider2D.bounds.center;
                moveToTarget(targetPoint);
            }
        }
        else
        {
            moveToPortal(targetRoom);
        }
    }

    private void moveToTarget(Vector3 targetPoint)
    {
        float distanceX = Mathf.Abs(targetPoint.x - transform.position.x);

        if (distanceX > 0.05f)
        {
            Vector3 movePoint = new Vector3(targetPoint.x, transform.position.y, transform.position.z);
            MoveTowards(movePoint);
        }
    }

    private void moveToPortal(RoomController targetRoom)
    {
        Portal portal = RoomManager.Instance.FindClosestPortal(CurrentRoom.Floor, targetRoom.Floor, transform.position);

        if (portal != null)
        {
            MoveTowards(portal.transform.position);

            if (Vector2.Distance(transform.position, portal.transform.position) < 0.1f)
            {
                portal.InteractPortal(transform, false);
            }
        }
    }
    public void MoveTowards(Vector3 target)
    {
        Vector3 current = transform.position;
        Vector3 targetPos = new Vector3(target.x, current.y, current.z);

        transform.position = Vector3.MoveTowards(current, targetPos, moveSpeed * Time.deltaTime);
    }

    public void SetMove(bool move) => CanMove = move; //TODO: 실제 멈추는 로직 필요
    private void setLastRoomEnterTime() => LastRoomEnterTime = Time.time;
    public bool IsArrived(RoomController targetRoom)
    {
        float targetX = targetRoom.Collider2D.bounds.center.x;
        float distanceX = Mathf.Abs(transform.position.x - targetX);
        return distanceX <= 0.2f;
    }

    public void SetLastKnowRoom(RoomController playerRoom) { LastKnownRoom = playerRoom; }
    public bool IsPlayerVisible() => CanSeePlayer;

    public void SetMoveSpeed(float speed) { moveSpeed = speed; }
}