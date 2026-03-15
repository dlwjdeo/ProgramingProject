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
    public virtual void OnTriggerStay2D(Collider2D other) { }
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
        if (enemy != null && enemy.IsDoorPauseActive)
        {
            enemy.Mover?.Stop();
            return true;
        }

        if (enemy?.EnemyVision == null) return false;
        if (!enemy.EnemyVision.TryGetDoorToOpen(out Door door)) return false;

        if (door.Open(enemy.DoorOpenDelay))
        {
            enemy.StartDoorPause(enemy.DoorOpenDelay);
            return true;
        }

        return false;
    }

    protected bool CanNavigate()
    {
        return enemy != null && GetEnemyRoom() != null && enemy.Mover != null;
    }

    protected RoomController GetEnemyRoom()
    {
        if (enemy == null) return null;
        if (enemy.CurrentRoom != null) return enemy.CurrentRoom;

        if (RoomManager.Instance != null && RoomManager.Instance.EnemyRoom != null)
        {
            enemy.SetCurrentRoom(RoomManager.Instance.EnemyRoom);
            return enemy.CurrentRoom;
        }

        return null;
    }

    protected bool MoveToRoomWithPortal(RoomController targetRoom, float arriveThreshold = 0.1f)
    {
        if (!CanNavigate()) return false;
        if (targetRoom == null) return false;

        RoomController enemyRoom = GetEnemyRoom();
        if (enemyRoom == null) return false;

        if (targetRoom.Floor == enemyRoom.Floor)
        {
            Vector3 center = targetRoom.Collider2D != null
                ? targetRoom.Collider2D.bounds.center
                : targetRoom.transform.position;

            float dir = enemy.Mover.MoveTowardsX(center, arriveThreshold);
            enemy.SetDirection(dir);

            return enemy.Mover.IsArrivedX(center, arriveThreshold);
        }

        Portal portal = RoomManager.Instance != null
            ? RoomManager.Instance.FindClosestPortal(enemyRoom.Floor, targetRoom.Floor, enemy.transform.position)
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

        RoomController enemyRoom = GetEnemyRoom();
        if (enemyRoom == null) return;

        Transform playerTf = null;
        RoomController playerRoom = null;

        if (enemy.CurrentChaseTarget.Type == ChaseTargetType.Player && enemy.CurrentChaseTarget.Transform != null)
        {
            playerTf = enemy.CurrentChaseTarget.Transform;
            playerRoom = enemy.CurrentChaseTarget.Room;
        }
        else
        {
            var player = Player;
            if (player == null) return;

            playerTf = player.transform;
            playerRoom = player.CurrentRoom;
        }

        if (playerRoom == null)
        {
            float dir = enemy.Mover.MoveTowardsX(playerTf.position, arriveThreshold);
            enemy.SetDirection(dir);
            return;
        }

        if (playerRoom.Floor == enemyRoom.Floor)
        {
            float dir = enemy.Mover.MoveTowardsX(playerTf.position, arriveThreshold);
            enemy.SetDirection(dir);
            return;
        }

        Portal portal = RoomManager.Instance != null
            ? RoomManager.Instance.FindClosestPortalByTargetX(enemyRoom.Floor, playerRoom.Floor, playerTf.position.x)
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
        if (targetRoom == null)
        {
            targetRoom = RoomManager.Instance != null ? RoomManager.Instance.GetRandomRoom(GetEnemyRoom()) : null;
            if (targetRoom == null) return;
        }

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
        if (targetRoom == null)
        {
            targetRoom = PlayerRoom;
            if (targetRoom == null && RoomManager.Instance != null)
            {
                targetRoom = RoomManager.Instance.PlayerRoom;
            }

            if (targetRoom == null)
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
                return;
            }
        }

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
        enemy.ClearChaseTarget();
    }

    public override void Update()
    {
        var player = Player;
        if (player == null)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Suspicious);
            return;
        }

        RoomController latestPlayerRoom = player.CurrentRoom;
        if (latestPlayerRoom == null && RoomManager.Instance != null)
        {
            latestPlayerRoom = RoomManager.Instance.PlayerRoom;
        }
        enemy.SetChaseTargetPlayer(player.transform, latestPlayerRoom);

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
        HandlePlayerContact(other);
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        HandlePlayerContact(other);
    }

    private void HandlePlayerContact(Collider2D other)
    {
        if (!other.CompareTag(TagName.Player)) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        if (player.IsHidden)
        {
            bool recentlyHidden = (Time.time - player.LastHideTime) < 2f;

            if (recentlyHidden)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
                Debug.Log("은신 직후 접촉: 플레이어 사망 처리");
            }
            else
            {
                Debug.Log("은신 상태: 플레이어를 감지할 수 없습니다.");
            }
            return;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        Debug.Log("Game Over: Caught by Enemy");
    }

    public override void Exit()
    {
        enemy.SetMoveMode(EnemyMoveMode.Walk);
        lostTimer = 0f;
        enemy.ClearChaseTarget();
    }
}
