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
    private float waitTime = 2f;
    private bool arrived;

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
        }

        enemy.MoveAI(targetRoom);

        if (!arrived && enemy.IsArrived(targetRoom))
        {
            arrived = true;
            waitTime = 2f;
        }

        if (arrived)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0f)
            {
                targetRoom = RoomManager.Instance.GetRandomRoom();
                Debug.Log("������ ����: " + targetRoom.name);
                arrived = false; 
            }
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
    private bool isPlayerVisible;
    private float chaseSpeed = 3f;

    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
        lostTimer = lostPlayerDelay;
        isPlayerVisible = true;
        enemy.SetMoveSpeed(chaseSpeed);
    }

    public override void Update()
    {
        RoomController playerRoom = Player.Instance.CurrentRoom;

        enemy.MoveAI(playerRoom, true);

        if (CanSeePlayer())
        {
            // �÷��̾� ��ġ ����
            enemy.SetLastKnowRoom(playerRoom);
            lostTimer = lostPlayerDelay;
            isPlayerVisible = true;
        }
        else
        {
            if (isPlayerVisible)
            {
                isPlayerVisible = false;
                enemy.SetLastKnowRoom(playerRoom);
            }

            lostTimer -= Time.deltaTime;
            if (lostTimer <= 0f )
            {
                enemy.StateMachine.ChangeState(enemy.StateMachine.Suspicious);
            }
        }
    }

    private bool CanSeePlayer()
    {
        if (Player.Instance == null)
            return false;

        var playerRoom = Player.Instance.CurrentRoom;
        if (playerRoom == null || playerRoom.Floor != enemy.CurrentRoom.Floor)
            return false;

        float distance = Vector2.Distance(enemy.transform.position, Player.Instance.transform.position);
        return distance <= enemy.ViewRange;
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
        isPlayerVisible = false;
        enemy.SetMoveSpeed(enemy.DefultMoveSpeed);
    }
}