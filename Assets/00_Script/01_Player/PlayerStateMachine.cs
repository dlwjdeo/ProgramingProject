public class PlayerStateMachine
{
    public PlayerIdleState Idle { get; private set; }
    public PlayerRunState Run { get; private set; }
    public PlayerJumpState Jump { get; private set; }
    public PlayerFallState Fall { get; private set; }
    public PlayerHideState Hide { get; private set; }

    private IPlayerState currentState;
    private Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;

        Idle = new PlayerIdleState(player);
        Run = new PlayerRunState(player);
        Jump = new PlayerJumpState(player);
        Fall = new PlayerFallState(player);
        Hide = new PlayerHideState(player);
        currentState = Idle; // 초기 상태
        currentState.Enter();
    }

    public void ChangeState(IPlayerState newState)
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