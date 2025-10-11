using UnityEngine;
public class EnemyStateMachine
{
    public EnemyPatrolState Patrol { get; private set; }
    public EnemyWaitState Wait { get; private set; }
    public EnemySuspiciousState Suspicious { get; private set; }
    public EnemyChaseState Chase { get; private set; }

    public EnemyState CurrentState { get; private set; }
    private Enemy enemy;

    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;

        Patrol = new EnemyPatrolState(enemy);
        Suspicious = new EnemySuspiciousState(enemy);
        Chase = new EnemyChaseState(enemy);
        Wait = new EnemyWaitState(enemy);

        CurrentState = Patrol; 
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
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