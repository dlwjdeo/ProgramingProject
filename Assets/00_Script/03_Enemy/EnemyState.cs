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

    protected bool TryOpenDoorIfNeeded()
    {
        if (enemy?.EnemyVision == null) return false;
        if (!enemy.EnemyVision.TryGetDoorToOpen(out Door door)) return false;

        door.Open(0.1f);
        return true;
    }

    protected bool CanNavigate()
    {
        return enemy != null && enemy.CurrentRoom != null && enemy.Mover != null;
    }

    protected bool MoveToRoomWithPortal(RoomController targetRoom, float arriveThreshold = 0.1f)
    {
        if (!CanNavigate()) return false;
        if (targetRoom == null) return false;

        if (targetRoom.Floor == enemy.CurrentRoom.Floor)
        {
            Vector3 center = targetRoom.Collider2D != null
                ? targetRoom.Collider2D.bounds.center
                : targetRoom.transform.position;

            float dir = enemy.Mover.MoveTowardsX(center, arriveThreshold);
            enemy.SetDirection(dir);

            return enemy.Mover.IsArrivedX(center, arriveThreshold);
        }

        Portal portal = RoomManager.Instance != null
            ? RoomManager.Instance.FindClosestPortal(enemy.CurrentRoom.Floor, targetRoom.Floor, enemy.transform.position)
            : null;

        if (portal == null) return false;

        float pdir = enemy.Mover.MoveTowardsX(portal.transform.position, arriveThreshold);
        enemy.SetDirection(pdir);

        if (enemy.Mover.IsArrivedX(portal.transform.position, arriveThreshold))
            portal.InteractPortal(enemy.transform, false);

        return false;
    }

    protected void MoveToPlayerWithPortal(float arriveThreshold = 0.05f)
    {
        if (!CanNavigate()) return;

        var player = Player;
        if (player == null) return;

        RoomController playerRoom = player.CurrentRoom;
        if (playerRoom == null)
        {
            float dir = enemy.Mover.MoveTowardsX(player.transform.position, arriveThreshold);
            enemy.SetDirection(dir);
            return;
        }

        if (playerRoom.Floor == enemy.CurrentRoom.Floor)
        {
            float dir = enemy.Mover.MoveTowardsX(player.transform.position, arriveThreshold);
            enemy.SetDirection(dir);
            return;
        }

        Portal portal = RoomManager.Instance != null
            ? RoomManager.Instance.FindClosestPortal(enemy.CurrentRoom.Floor, playerRoom.Floor, enemy.transform.position)
            : null;

        if (portal == null) return;

        float pdir = enemy.Mover.MoveTowardsX(portal.transform.position, arriveThreshold);
        enemy.SetDirection(pdir);

        if (enemy.Mover.IsArrivedX(portal.transform.position, arriveThreshold))
            portal.InteractPortal(enemy.transform, false);
    }
}

public class EnemyPatrolState : EnemyState
{
    private RoomController targetRoom;

    public EnemyPatrolState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Patrol);
        enemy.SetMoveMode(EnemyMoveMode.Walk);

        targetRoom = RoomManager.Instance != null ? RoomManager.Instance.GetRandomRoom(enemy.CurrentRoom) : null;
    }

    public override void Update()
    {
        if (TryChaseIfPlayerVisible()) return;
        if (TryOpenDoorIfNeeded()) return;
        if (!CanNavigate()) return;
        if (targetRoom == null) return;

        bool arrived = MoveToRoomWithPortal(targetRoom, 0.1f);
        if (arrived)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Idle);
            return;
        }
    }

    public override void Exit()
    {
        targetRoom = null;
    }
}

public class EnemyIdleState : EnemyState
{
    private float waitTime;

    public EnemyIdleState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Idle);
        waitTime = 2f;

        enemy.Mover?.Stop();
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

    private bool arrived;

    public EnemySuspiciousState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Suspicious);
        enemy.SetMoveMode(EnemyMoveMode.Walk);

        targetRoom = PlayerRoom;
        arrived = false;
    }

    public override void Update()
    {
        if (TryChaseIfPlayerVisible()) return;
        if (TryOpenDoorIfNeeded()) return;
        if (!CanNavigate()) return;
        if (targetRoom == null) return;

        if (!arrived)
        {
            arrived = MoveToRoomWithPortal(targetRoom, 0.1f);
            if (arrived)
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Idle);
            }
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
    private const float LostPlayerDelay = 10f;
    private float lostTimer;

    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
        enemy.SetMoveMode(EnemyMoveMode.Run);

        lostTimer = LostPlayerDelay;
    }

    public override void Update()
    {
        var player = Player;
        if (player == null) return;
        if (!CanNavigate()) return;

        if (enemy.EnemyVision != null && enemy.EnemyVision.IsPlayerVisible)
        {
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

        if (TryOpenDoorIfNeeded()) return;

        MoveToPlayerWithPortal(0.05f);
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
                Debug.Log("��Ŵ ���ӿ���");
            }
            else
            {
                Debug.Log("�ν� �Ұ�");
            }
            return;
        }

        GameManager.Instance.GameOver();
        Debug.Log("Game Over: Caught by Enemy");
    }

    public override void Exit()
    {
        enemy.SetMoveMode(EnemyMoveMode.Walk);
        lostTimer = 0f;
    }
}
