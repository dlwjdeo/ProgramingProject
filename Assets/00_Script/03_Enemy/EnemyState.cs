using Unity.VisualScripting;
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

    public override void Enter()
    {
        enemy.SetStateType(EnemyStateType.Patrol);
    }

    public override void Update()
    {

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
                    Debug.Log("무시");

                    enemy.StateMachine.ChangeState(enemy.StateMachine.Chase);
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
        var playerRoom = RoomManager.Instance.GetPlayerRoom();
        var enemyRoom = RoomManager.Instance.GetEnemyRoom();
        if (playerRoom == null || enemyRoom == null) return;

        // 같은 층이면 플레이어 실제 위치 기준으로 추적
        if (playerRoom.Floor == enemyRoom.Floor)
        {
            float playerX = Player.Instance._Collider2D.bounds.center.x;
            float enemyX = enemy.transform.position.x;

            if (Mathf.Abs(playerX - enemyX) > 0.05f)
            {
                Vector3 target = new Vector3(playerX, enemy.transform.position.y, enemy.transform.position.z);
                enemy.MoveTowards(target); 
            }
        }
        else
        {
            // 다른 층이면 포탈 찾아 이동
            Portal portal = RoomManager.Instance.FindClosestPortal(enemyRoom.Floor, playerRoom.Floor, enemy.transform.position);

            if (portal != null)
            {
                enemy.MoveTowards(portal.transform.position);

                if (Vector2.Distance(enemy.transform.position, portal.transform.position) < 0.1f)
                {
                    portal.MoveThroughPortal(enemy.transform, false);
                }
            }
        }
    }
    public override void Exit()
    {
        
    }
}