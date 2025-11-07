using UnityEngine;

public class PlayerStateMachine
{
    public PlayerIdleState Idle { get; private set; }
    public PlayerWalkState Walk { get; private set; }
    public PlayerJumpState Jump { get; private set; }
    public PlayerFallState Fall { get; private set; }
    public PlayerHideState Hide { get; private set; }
    public PlayerRunState Run { get; private set; } 

    private IState currentState;
    private Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;

        Idle = new PlayerIdleState(player);
        Walk = new PlayerWalkState(player);
        Jump = new PlayerJumpState(player);
        Fall = new PlayerFallState(player);
        Hide = new PlayerHideState(player);
        Run = new PlayerRunState(player);

        currentState = Idle; // 초기 상태
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