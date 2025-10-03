public class EnemyStateMachine
{
    public EnemyPatrolState Patrol { get; private set; }
    public EnemySuspiciousState Suspicious { get; private set; }
    public EnemyChaseState Chase { get; private set; }

    private EnemyState currentState;
    private Enemy enemy;

    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;

        Patrol = new EnemyPatrolState(enemy);
        Suspicious = new EnemySuspiciousState(enemy);
        Chase = new EnemyChaseState(enemy);

        currentState = Patrol; // 초기 상태
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        Debug.Log($"[EnemyStateMachine] {currentState?.GetType().Name ?? "None"} -> {newState.GetType().Name}");

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}