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
        if (enemy.IsPlayerVisible())
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            return;
        }

        enemy.MoveAI(targetRoom);

        if (enemy.IsArrived(targetRoom))
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Wait);
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
        if (enemy.IsPlayerVisible())
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
            return;
        }

        waitTime -= Time.deltaTime;
        if (waitTime <= 0f)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
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

        // ���� �� �÷��̾ ������� �� ��ϵ� ������ ������ �̵�
        targetRoom = enemy.LastKnownRoom;
        arrived = false;
        timer = suspicionTime;
    }

    public override void Update()
    {
        if (enemy.IsPlayerVisible())
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
        }

        // ���� ���� ���̸� �̵�
        if (!arrived)
        {
            enemy.MoveAI(targetRoom);

            if (enemy.IsArrived(targetRoom))
            {
                arrived = true;
                timer = suspicionTime;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
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
    private float lostPlayerDelay = 10f;     // �÷��̾ ��ģ �� ��� �ð�
    private float lostTimer;
    private bool prevVisible;
    private float chaseSpeed = 3f;

    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
        lostTimer = lostPlayerDelay;
        prevVisible = true;
        enemy.SetMoveSpeed(chaseSpeed);
    }

    public override void Update()
    {
        RoomController playerRoom = Player.Instance.CurrentRoom;

        enemy.MoveAI(playerRoom, true);

        if (enemy.IsPlayerVisible())
        {
            // �÷��̾� ��ġ ����
            enemy.SetLastKnowRoom(playerRoom);
            lostTimer = lostPlayerDelay;
            prevVisible = true;
        }
        else
        {
            if (prevVisible)
            {
                prevVisible = false;
                enemy.SetLastKnowRoom(playerRoom);
            }

            lostTimer -= Time.deltaTime;
            if (lostTimer <= 0f )
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Suspicious);
            }
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
                float diff = enemy.LastRoomEnterTime - player.LastHideTime;
                if (diff < 1f)
                {
                    Debug.Log("��Ŵ ���ӿ���");
                }
                else
                {
                    Debug.Log("�ν� �Ұ�");
                }
            }
            else
            {
                Debug.Log("���� ����");
            }
        }
    }

    public override void Exit()
    {
        lostTimer = 0f;
        prevVisible = false;
        enemy.SetMoveSpeed(enemy.DefultMoveSpeed);
    }
}