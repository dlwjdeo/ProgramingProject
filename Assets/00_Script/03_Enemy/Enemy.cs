using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;

    public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private EnemyStateType state;

    public bool CanMove { get; private set; }

    private BoxCollider2D detectCollier;

    public float LastRoomEnterTime { get; private set; }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this);
        detectCollier = GetComponentInChildren<BoxCollider2D>();
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StateMachine.CurrentState?.OnTriggerEnter2D(collision);
    }
    public void SetStateType(EnemyStateType type)
    {
        state = type;
    }

    public void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void SetMove(bool move) => CanMove = move;
    public void SetLastRoomEnterTime() => LastRoomEnterTime = Time.time;
}