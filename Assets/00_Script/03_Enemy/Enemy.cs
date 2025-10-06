using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;

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
        UpdateCurrentRoom();
        StateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }

    private void UpdateCurrentRoom()
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
    public void SetStateType(EnemyStateType type)
    {
        state = type;
    }
    public void MoveAI(RoomController targetRoom)
    {
        if (targetRoom == null || CurrentRoom == null)
            return;

        // 단순히 넘겨받은 Room 기준으로만 이동 (상태에서 판단)
        if (targetRoom.Floor == CurrentRoom.Floor)
        {
            Vector3 targetPoint = targetRoom.Collider2D.bounds.center;
            moveToTarget(targetPoint);
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
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void SetMove(bool move) => CanMove = move; //TODO: 실제 멈추는 로직 필요
    private void setLastRoomEnterTime() => LastRoomEnterTime = Time.time;
    public bool IsArrived(RoomController targetRoom)
    {
        float targetX = targetRoom.Collider2D.bounds.center.x;
        float distanceX = Mathf.Abs(transform.position.x - targetX);
        return distanceX <= 0.2f;
    }
}