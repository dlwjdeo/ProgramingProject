using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float moveSpeed = 2f;

    public EnemyStateMachine StateMachine { get; private set; }
    [SerializeField] private EnemyStateType state;

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public void SetStateType(EnemyStateType type)
    {
        state = type;
    }

    public void MoveTowards(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void Teleport(Vector3 target)
    {
        transform.position = target;
    }
}