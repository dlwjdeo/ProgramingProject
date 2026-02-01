using UnityEngine;

public class PlayerStateMachine
{
    public PlayerIdleState Idle { get; private set; }
    public PlayerWalkState Walk { get; private set; }
    public PlayerHideState Hide { get; private set; }
    public PlayerRunState Run { get; private set; }

    private IState currentState;
    private readonly Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;

        Idle = new PlayerIdleState(player);
        Walk = new PlayerWalkState(player);
        Hide = new PlayerHideState(player);
        Run = new PlayerRunState(player);

        currentState = Idle;
        currentState.Enter();
    }

    public void ChangeState(IState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
