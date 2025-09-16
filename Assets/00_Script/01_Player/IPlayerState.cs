using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerMover player);
    void Update(PlayerMover player);
    void Exit(PlayerMover player);
}

public class PlayerIdleState : IPlayerState
{
    public void Enter(PlayerMover player) { }
    public void Update(PlayerMover player)
    {
        if (Mathf.Abs(player.Move.x) > 0.01f)
            player.StateMachine.ChangeState(player.StateMachine.Run, player);

        if (!player.GroundChecker.IsGrounded)
            player.StateMachine.ChangeState(player.StateMachine.Fall, player);
    }
    public void Exit(PlayerMover player) { }
}

public class PlayerRunState : IPlayerState
{
    public void Enter(PlayerMover player) { }
    public void Update(PlayerMover player)
    {
        if (Mathf.Abs(player.Move.x) < 0.01f)
            player.StateMachine.ChangeState(player.StateMachine.Idle, player);

        if (!player.GroundChecker.IsGrounded)
            player.StateMachine.ChangeState(player.StateMachine.Fall, player);
    }
    public void Exit(PlayerMover player) { }
}

public class PlayerJumpState : IPlayerState
{
    public void Enter(PlayerMover player)
    {
        player.DoJump();
    }
    public void Update(PlayerMover player)
    {
        if (player._rigidbody2D.velocity.y < 0)
            player.StateMachine.ChangeState(player.StateMachine.Fall, player);
    }
    public void Exit(PlayerMover player) { }
}

public class PlayerFallState : IPlayerState
{
    public void Enter(PlayerMover player) { }
    public void Update(PlayerMover player)
    {
        if (player.GroundChecker.IsGrounded)
            player.StateMachine.ChangeState(player.StateMachine.Idle, player);
    }
    public void Exit(PlayerMover player) { }
}
