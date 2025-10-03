using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStateMachine StateMachine { get; private set; }

    private void Awake()
    {
        StateMachine = new EnemyStateMachine(this);
    }

    private void Update()
    {
        StateMachine.Update();
    }

    public void MoveTowards(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}