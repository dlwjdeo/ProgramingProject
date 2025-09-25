using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public abstract class PlayerState : IPlayerState
{
    protected Player player;

    protected PlayerState(Player player)
    {
        this.player = player;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter() { }

    public override void Update()
    {
        if (Mathf.Abs(player.PlayerMover.Move.x) > 0.01f)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Run);

        if (!player.PlayerMover.GroundChecker.IsGrounded)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
    }

    public override void Exit() { }
}

public class PlayerRunState : PlayerState
{
    public PlayerRunState(Player player) : base(player) { }

    public override void Enter() { }

    public override void Update()
    {
        if (Mathf.Abs(player.PlayerMover.Move.x) < 0.01f)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);

        if (!player.PlayerMover.GroundChecker.IsGrounded)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
    }

    public override void Exit() { }
}

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        player.PlayerMover.DoJump();
    }

    public override void Update()
    {
        if (player.Rigidbody2D.velocity.y < 0)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
    }

    public override void Exit() { }
}

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player) : base(player) { }

    public override void Enter() { }

    public override void Update()
    {
        if (player.PlayerMover.GroundChecker.IsGrounded)
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
    }

    public override void Exit() { }
}
public class PlayerHideState : PlayerState
{
    private float hideBuffer = 0.2f;
    private float timer;

    public PlayerHideState(Player player) : base(player) { }

    public override void Enter()
    {
        player.PlayerMover.SetMove(false);
        timer = hideBuffer;
    }

    public override void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (player.PlayerInputReader.InterationPressed)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
        }
    }

    public override void Exit()
    {
        player.PlayerMover.SetMove(true);
    }
}

