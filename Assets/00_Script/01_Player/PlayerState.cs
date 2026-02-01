using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
public abstract class PlayerState : IState
{
    protected readonly Player player;

    protected PlayerState(Player player)
    {
        this.player = player;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    protected void ApplyMoveInput()
    {
        Vector2 move = player.PlayerInputReader.GetMove();
        player.PlayerMover.SetMoveInput(move);

        if (Mathf.Abs(move.x) > 0.01f)
        {
            player.SetFacing(move.x > 0 ? 1 : -1);
        }
    }
}


public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Idle);
        player.PlayerMover.SetMoveEnabled(true);
        player.PlayerMover.SetSpeedMultiplier(1f);
        player.PlayerMover.SetMoveInput(Vector2.zero);
    }

    public override void Update()
    {
        player.PlayerStamina.Recover(Time.deltaTime);

        ApplyMoveInput();

        float mx = Mathf.Abs(player.PlayerMover.MoveInput.x);

        if (mx > 0.01f)
        {
            // �޸��� �Է��̸� Run, �ƴϸ� Walk
            if (player.PlayerInputReader.RunPressed && !player.PlayerStamina.IsEmpty)
                player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Run);
            else
                player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Walk);

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
        player.PlayerMover.SetMoveEnabled(true);
        player.PlayerMover.SetSpeedMultiplier(1f);
    }

    public override void Update()
    {
        // 버튼을 뗐을 때 플래그 초기화
        if (!player.PlayerInputReader.RunPressed)
        {
            player.SetStaminaDepleted(false);
        }

        player.PlayerStamina.Recover(Time.deltaTime);

        ApplyMoveInput();

        float mx = Mathf.Abs(player.PlayerMover.MoveInput.x);

        if (mx < 0.01f)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }

        // 홀드 중 스테미나 부족으로 인한 재진입 방지
        if (player.PlayerInputReader.RunPressed && player.PlayerStamina.CanRun && !player.IsStaminaDepleted)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Run);
            return;
        }
    }

    public override void Exit() { }
}

public class PlayerRunState : PlayerState
{
    private const float RunMultiplier = 1.7f;

    public PlayerRunState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Run);
        player.PlayerMover.SetMoveEnabled(true);
        player.PlayerMover.SetSpeedMultiplier(RunMultiplier);
    }

    public override void Update()
    {
        ApplyMoveInput();

        if (Mathf.Abs(player.PlayerMover.MoveInput.x) < 0.01f)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }

        if (!player.PlayerInputReader.RunPressed || !player.PlayerStamina.CanRun)
        {
            // 스테미나 부족으로 인해 Walk로 전환되면 플래그 설정 (홀드 중 재진입 방지)
            if (!player.PlayerStamina.CanRun)
            {
                player.SetStaminaDepleted(true);
            }
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Walk);
            return;
        }

        player.PlayerStamina.Decrease(Time.deltaTime);
    }

    public override void Exit()
    {
        player.PlayerMover.SetSpeedMultiplier(1f);
        // 입력으로 빠져나간 경우 플래그 초기화
        if (!player.PlayerInputReader.RunPressed)
        {
            player.SetStaminaDepleted(false);
        }
    }
}

public class PlayerHideState : PlayerState
{
    private float hideBuffer = 0.2f;
    private float timer;

    public PlayerHideState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Hide);
        player.PlayerMover.SetMoveEnabled(false);
        player.PlayerMover.SetMoveInput(Vector2.zero);

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
        player.PlayerMover.SetMoveEnabled(true);
        player.SetHidden(false);
    }
}

