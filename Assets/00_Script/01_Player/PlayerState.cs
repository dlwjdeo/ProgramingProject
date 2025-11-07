using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public abstract class PlayerState : IState
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

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Idle);
    }

    public override void Update()
    {
        player.PlayerStamina.Recover(Time.deltaTime);

        if (Mathf.Abs(player.PlayerMover.Move.x) > 0.01f)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Walk);
            return;
        }

        if (!player.PlayerMover.GroundChecker.IsGrounded)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
            return;
        }
    }

    public override void Exit() { }
}

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Walk);
    }

    public override void Update()
    {
        player.PlayerStamina.Recover(Time.deltaTime);

        if (Mathf.Abs(player.PlayerMover.Move.x) < 0.01f)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }

        if (!player.PlayerMover.GroundChecker.IsGrounded)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
            return;
        }

        if (player.PlayerInputReader.RunPressed && !player.PlayerStamina.IsEmpty)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Run);
            return;
        }
    }

    public override void Exit() { }
}

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter()
    {
        player.PlayerMover.DoJump();
        player.SetStateType(PlayerStateType.Jump);
        player.PlayerStamina.Consume(10f);
    }

    public override void Update()
    {

        if (player.Rigidbody2D.velocity.y < 0)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Fall);
            return;
        }
    }

    public override void Exit() { }
}

public class PlayerFallState : PlayerState
{
    public PlayerFallState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Fall);
    }

    public override void Update()
    {
        if (player.PlayerMover.GroundChecker.IsGrounded)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }
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
        player.SetStateType(PlayerStateType.Hide);
        player.PlayerMover.SetMove(false);
        timer = hideBuffer;
        player.SetHidden(true);
    }

    public override void Update()
    {
        player.PlayerStamina.Recover(Time.deltaTime);

        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            return;
        }

        if (player.PlayerInputReader.InterationPressed)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }
    }

    public override void Exit()
    {
        player.PlayerMover.SetMove(true);
        player.SetHidden(false);
    }
}

public class PlayerRunState : PlayerState
{
    private PlayerStamina stamina;
    private PlayerMover mover;

    public PlayerRunState(Player player) : base(player)
    {
        player.SetStateType(PlayerStateType.Run);
        stamina = player.PlayerStamina;
        mover = player.PlayerMover;
    }

    public override void Enter()
    {
        mover.SetSpeedMultiplier(1.7f); // 기본 속도의 1.7배
    }

    public override void Update()
    {
        if (!player.PlayerInputReader.RunPressed || stamina.IsEmpty)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }

        stamina.Decrease(Time.deltaTime);
    }

    public override void Exit()
    {
        mover.SetSpeedMultiplier(1f);
    }
}

