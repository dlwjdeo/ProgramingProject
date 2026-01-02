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

        if (RoomManager.Instance.EnemyRoom != null)
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

    public void SetStateType(EnemyStateType type) => state = type;

    public void SetMove(bool move)
    {
        if (Mover != null) Mover.SetMoveEnabled(move);
    }

    public void SetMoveSpeed(float speed)
    {
        if (Mover != null) Mover.MoveSpeed = speed;
    }

    public float Direction => (Mover != null) ? Mover.Direction : 1f;

    public void SetCurrentRoom(RoomController room)
    {
        CurrentRoom = room;
    }

    public void MoveToRoom(RoomController targetRoom)
    {
        if (targetRoom == null || CurrentRoom == null || Mover == null) return;

        if (targetRoom.Floor == CurrentRoom.Floor)
        {
            Vector3 targetPoint = targetRoom.Collider2D.bounds.center;
            Mover.MoveTowardsX(targetPoint);
        }
        else
        {
            MoveToPortal(targetRoom);
        }
    }

    public void MoveToPortal(RoomController targetRoom)
    {
        if (targetRoom == null || CurrentRoom == null || Mover == null) return;

        Portal portal = RoomManager.Instance.FindClosestPortal(CurrentRoom.Floor, targetRoom.Floor, transform.position);
        if (portal == null) return;

        Mover.MoveTowardsX(portal.transform.position);

        if (Mover.IsArrivedX(portal.transform.position))
            portal.InteractPortal(transform, false);
    }
}
