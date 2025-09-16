public class PlayerStateMachine
{
    public readonly PlayerIdleState Idle = new PlayerIdleState();
    public readonly PlayerRunState Run = new PlayerRunState();
    public readonly PlayerJumpState Jump = new PlayerJumpState();
    public readonly PlayerFallState Fall = new PlayerFallState();

    private IPlayerState currentState;

    public void ChangeState(IPlayerState newState, PlayerMover player)
    {
        if (currentState == newState) return; 

        currentState?.Exit(player);
        currentState = newState;
        currentState.Enter(player);
    }

    public void Update(PlayerMover player)
    {
        currentState?.Update(player);
    }
}