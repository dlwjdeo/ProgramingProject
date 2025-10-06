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
                Debug.Log("목적지 변경: " + targetRoom.name);
                arrived = false; // 리셋
            }
        }
    }

    public override void Exit()
    {

    }
}

public class EnemySuspiciousState : EnemyState
{
    private float suspicionTime = 2f;
    private float timer;

    public EnemySuspiciousState(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Suspicious);
        timer = suspicionTime;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            enemy.StateMachine.ChangeState(enemy.StateMachine.Patrol);
        }

    }

    public override void Exit()
    {

    }
}

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(Enemy enemy) : base(enemy) { }

    private bool canFindPlayer;

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(TagName.Player))
        {
            Player player = other.GetComponent<Player>();
            if (player.IsHidden)
            {
                float diff = enemy.LastRoomEnterTime - player.LastHideTime;
                if (diff < 1f)
                {
                    Debug.Log("들킴");
                }
                else
                {
                    Debug.Log("안들킴");
                }
            }
            else
            {
                // 숨어있지 않음
            }
        }
    }
    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Chase);
    }

    public override void Update()
    {
        enemy.MoveAI(Player.Instance.CurrentRoom);
    }
    public override void Exit()
    {
        
    }
}