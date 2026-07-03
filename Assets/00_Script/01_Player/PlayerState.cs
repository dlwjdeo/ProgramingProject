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
        player.PlayerAnimator.CrossFade("Idle", 0.1f);
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
        player.PlayerAnimator.CrossFade("Walk", 0.1f);
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

        // 발걸음 요청 (재생 간격/클립 선택은 SoundManager에서 처리)
        if (SoundManager.Instance != null)
            SoundManager.Instance.RequestPlayerFootstep(player.transform, false);

        // 홀드 중 스테미나 부족으로 인한 재진입 방지
        if (player.PlayerInputReader.RunPressed && player.PlayerStamina.CanRun && !player.IsStaminaDepleted)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Run);
            return;
        }
    }

    public override void Exit()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.ClearPlayerFootstep(player.transform);
    }
}

public class PlayerRunState : PlayerState
{
    public PlayerRunState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Run);
        player.PlayerMover.SetMoveEnabled(true);
        player.PlayerMover.SetSpeedMultiplier(player.PlayerMover.RunSpeedMultiplier);
        player.PlayerAnimator.CrossFade("Run", 0.1f);
    }

    public override void Update()
    {
        ApplyMoveInput();

        if (Mathf.Abs(player.PlayerMover.MoveInput.x) < 0.01f)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Idle);
            return;
        }

        // 발걸음 요청 (재생 간격/클립 선택은 SoundManager에서 처리)
        if (SoundManager.Instance != null)
            SoundManager.Instance.RequestPlayerFootstep(player.transform, true);

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
        
        if (SoundManager.Instance != null)
            SoundManager.Instance.ClearPlayerFootstep(player.transform);
        
        // 입력으로 빠져나간 경우 플래그 초기화
        if (!player.PlayerInputReader.RunPressed)
        {
            player.SetStaminaDepleted(false);
        }
    }
}

public class PlayerHideState : PlayerState
{
    private const string HideClipName = "Sit";

    private float hideBuffer = 0.2f;
    private float timer;
    private bool isWaitingLoopRestart;
    private float loopRestartTimer;

    public Action OnEnterHideState;
    public Action OnExitHideState;

    public PlayerHideState(Player player) : base(player) { }

    public override void Enter()
    {
        player.SetStateType(PlayerStateType.Hide);
        player.PlayerMover.SetMoveEnabled(false);
        player.PlayerMover.SetMoveInput(Vector2.zero);

        timer = hideBuffer;
        isWaitingLoopRestart = false;
        loopRestartTimer = 0f;
        player.SetHidden(true);
        player.PlayerAnimator.speed = 1f;
        player.PlayerAnimator.CrossFade(HideClipName, 0.1f);
        player.PlayerRenderer.material.color = player.HideColor;
        OnEnterHideState?.Invoke();
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

        HandleHideLoop();
    }

    public override void Exit()
    {
        player.PlayerAnimator.speed = 1f;
        player.PlayerMover.SetMoveEnabled(true);
        player.SetHidden(false);
        player.PlayerRenderer.material.color = Color.white;
        OnExitHideState?.Invoke();
    }

    private void HandleHideLoop()
    {
        Animator animator = player.PlayerAnimator;
        if (animator == null)
            return;

        if (isWaitingLoopRestart)
        {
            loopRestartTimer -= Time.deltaTime;
            if (loopRestartTimer <= 0f)
            {
                isWaitingLoopRestart = false;
                animator.speed = 1f;
                animator.Play(HideClipName, 0, 0f);
            }
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!stateInfo.IsName(HideClipName))
            return;

        if (stateInfo.normalizedTime < 1f)
            return;

        // Keep the pose at the first frame until a random delay ends.
        animator.Play(HideClipName, 0, 0f);
        animator.speed = 0f;

        float minDelay = Mathf.Max(0f, player.HideLoopPauseMin);
        float maxDelay = Mathf.Max(minDelay, player.HideLoopPauseMax);
        loopRestartTimer = UnityEngine.Random.Range(minDelay, maxDelay);
        isWaitingLoopRestart = true;
    }
}

