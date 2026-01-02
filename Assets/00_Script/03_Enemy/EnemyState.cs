using UnityEngine;

public abstract class EnemyState : IState
{
    protected readonly Enemy enemy;

    protected EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }

    protected Player Player => Player.Instance;
    protected RoomController PlayerRoom => Player != null ? Player.CurrentRoom : null;

    protected bool TryChaseIfPlayerVisible()
    {
        if (enemy.EnemyVision != null && enemy.EnemyVision.IsPlayerVisible)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            return true;
        }
        return false;
    }

    protected bool CanNavigate()
    {
        return enemy != null && enemy.CurrentRoom != null;
    }
}

public class EnemyPatrolState : EnemyState
{
    private RoomController targetRoom;

    public EnemyPatrolState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Patrol);
        enemy.SetMoveSpeed(enemy.DefaultMoveSpeed);

        targetRoom = RoomManager.Instance != null ? RoomManager.Instance.GetRandomRoom() : null;
    }

    public override void Update()
    {
        if (TryChaseIfPlayerVisible()) return;
        if (!CanNavigate()) return;
        if (targetRoom == null) return;

        enemy.MoveToRoom(targetRoom);

        var center = targetRoom.Collider2D.bounds.center;
        if (enemy.Mover != null && enemy.Mover.IsArrivedX(center))
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Wait);
            return;
        }
    }

    public override void Exit() { }
}

public class EnemyWaitState : EnemyState
{
    private float waitTime;

    public EnemyWaitState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Wait);
        waitTime = 2f;
    }

    public override void Update()
    {
        if (TryChaseIfPlayerVisible()) return;

        waitTime -= Time.deltaTime;
        if (waitTime <= 0f)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
            return;
        }
    }

    public override void Exit() { }
}

public class EnemySuspiciousState : EnemyState
{
    private RoomController targetRoom;
    private float timer;

    private const float SuspicionTime = 2f;
    private bool arrived;

    public EnemySuspiciousState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Suspicious);
        enemy.SetMoveSpeed(enemy.DefaultMoveSpeed);

        targetRoom = PlayerRoom;
        arrived = false;
        timer = SuspicionTime;
    }

    public override void Update()
    {
        if (TryChaseIfPlayerVisible()) return;
        if (!CanNavigate()) return;
        if (targetRoom == null) return;

        if (!arrived)
        {
            enemy.MoveToRoom(targetRoom);

            var center = targetRoom.Collider2D.bounds.center;
            if (enemy.Mover != null && enemy.Mover.IsArrivedX(center))
            {
                arrived = true;
                timer = SuspicionTime;
            }
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
            return;
        }
    }

    public override void Exit()
    {
        targetRoom = null;
        arrived = false;
    }
}

public class EnemyChaseState : EnemyState
{
    private const float ChaseSpeed = 3f;

    private const float LostPlayerDelay = 10f;
    private float lostTimer;

    private float lastSeenTime;

    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
        enemy.SetMoveSpeed(ChaseSpeed);

        lostTimer = LostPlayerDelay;
        lastSeenTime = Time.time;
    }

    public override void Update()
    {
        var player = Player;
        if (player == null) return;
        if (!CanNavigate()) return;

        RoomController playerRoom = player.CurrentRoom;
        if (playerRoom == null) return;

        if (enemy.EnemyVision != null && enemy.EnemyVision.IsPlayerVisible)
        {
            lastSeenTime = Time.time;
            lostTimer = LostPlayerDelay;
        }
        else
        {
            lostTimer -= Time.deltaTime;
            if (lostTimer <= 0f)
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Suspicious);
                return;
            }
        }

        if (playerRoom.Floor == enemy.CurrentRoom.Floor)
        {
            enemy.Mover?.MoveTowardsX(player.transform.position);
        }
        else
        {
            enemy.MoveToPortal(playerRoom);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(TagName.Player)) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (player.IsHidden)
        {
            bool recentlyHidden = (Time.time - player.LastHideTime) < 1f;

            if (recentlyHidden)
            {
                Debug.Log("들킴 게임오버");
            }
            else
            {
                Debug.Log("인식 불가");
            }
            return;
        }

        GameManager.Instance.GameOver();
    }

    public override void Exit()
    {
        enemy.SetMoveSpeed(enemy.DefaultMoveSpeed);
        lostTimer = 0f;
    }
}