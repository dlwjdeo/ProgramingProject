using UnityEngine;

public class EnemyStateMachine
{
    public EnemyPatrolState Patrol { get; private set; }
    public EnemyIdleState Idle { get; private set; }
    public EnemySuspiciousState Suspicious { get; private set; }
    public EnemyChaseState Chase { get; private set; }

    public EnemyState CurrentState { get; private set; }
    private readonly Enemy enemy;

    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;

        Patrol = new EnemyPatrolState(enemy);
        Idle = new EnemyIdleState(enemy);
        Suspicious = new EnemySuspiciousState(enemy);
        Chase = new EnemyChaseState(enemy);

        CurrentState = Patrol;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        if (newState == null) return;
        if (CurrentState == newState) return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}
