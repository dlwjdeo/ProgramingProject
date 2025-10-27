using UnityEngine;
public abstract class EnemyState : IState
{
    protected Enemy enemy;

    protected EnemyState(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

    public virtual void OnTriggerEnter2D(Collider2D other) { }
    public virtual void OnTriggerExit2D(Collider2D other) { }
}

public class EnemyPatrolState : EnemyState
{
    public EnemyPatrolState(Enemy enemy) : base(enemy) { }
    private RoomController targetRoom;

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Patrol);
        targetRoom = RoomManager.Instance.GetRandomRoom();
    }

    public override void Update()
    {
        if (enemy.EnemyVision.IsPlayerVisible)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            Debug.Log("발견");
            return;
        }

        if (targetRoom == null) return;

        if (targetRoom.Floor == enemy.CurrentRoom.Floor)
        {
            Vector3 targetPoint = targetRoom.Collider2D.bounds.center;
            enemy.MoveTowards(targetPoint);

            if (enemy.IsArrived(targetPoint))
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Wait);
                return;
            }
        }
        else
        {
            enemy.MoveToPortal(targetRoom);
        }
    }

    public override void Exit()
    {

    }
}

public class EnemyWaitState : EnemyState
{
    public EnemyWaitState(Enemy enemy) : base(enemy) { }
    private float waitTime = 2f;

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Wait);
        waitTime = 2f;
    }

    public override void Update()
    {
        if (enemy.EnemyVision.IsPlayerVisible)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            return;
        }

        waitTime -= Time.deltaTime;
        if (waitTime <= 0f)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
            return;
        }
    }

    public override void Exit()
    {
        
    }
}

public class EnemySuspiciousState : EnemyState
{
    private RoomController targetRoom;
    private float suspicionTime = 2f;
    private float timer;
    private bool arrived;

    public EnemySuspiciousState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Suspicious);
        targetRoom = Player.Instance.CurrentRoom; //의심상태로 진입 시 항상 플레이어의 현재 방으로 탐색
        arrived = false;
        timer = suspicionTime;
    }

    public override void Update()
    {
        if (enemy.EnemyVision.IsPlayerVisible)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            return;
        }

        if (targetRoom == null) return;

        if (targetRoom.Floor == enemy.CurrentRoom.Floor)
        {
            Vector3 center = targetRoom.Collider2D.bounds.center;
            enemy.MoveTowards(center);

            if (enemy.IsArrived(center) && !arrived)
            {
                arrived = true;
                timer = suspicionTime;
            }
        }
        else
        {
            enemy.MoveToPortal(targetRoom);
        }

        if (arrived)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
                return;
            }
        }
    }

    public override void Exit()
    {
        arrived = false;
        targetRoom = null;
    }
}

public class EnemyChaseState : EnemyState
{
    private float lostPlayerDelay = 10f;
    private float lostTimer;
    private float chaseSpeed = 3f;
    private float lastDetectTime;
    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
        enemy.SetMoveSpeed(chaseSpeed);
        lostTimer = lostPlayerDelay;
    }

    public override void Update()
    {
        if (Player.Instance == null) return;
        RoomController playerRoom = Player.Instance.CurrentRoom;

        if (playerRoom == null || enemy.CurrentRoom == null)
            return;

        if (enemy.EnemyVision.IsPlayerVisible)
        {
            lastDetectTime = Time.time;
            lostTimer = lostPlayerDelay;
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
            enemy.MoveTowards(Player.Instance.transform.position);
        }
        else
        {
            enemy.MoveToPortal(playerRoom);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagName.Player))
        {
            Player player = other.GetComponent<Player>();

            if (player == null) return;

            if (player.IsHidden)
            {
                float diff = player.LastHideTime - lastDetectTime;
                if (diff < 1f)
                {
                    Debug.Log("들킴 게임오버");
                }
                else
                {
                    Debug.Log("인식 불가");
                }
            }
            else
            {
                Debug.Log("게임 오버");
            }
        }
    }

    public override void Exit()
    {
        lostTimer = 0f;
        enemy.SetMoveSpeed(enemy.DefultMoveSpeed);
    }
}