using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float detectInterval = 0.1f;

    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private LayerMask playerMask;

    public bool IsPlayerVisible { get; private set; }

    private Enemy enemy;
    private Coroutine detectRoutine;

    private LayerMask combinedMask;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        combinedMask = obstacleMask | playerMask;
    }

    private void OnEnable()
    {
        detectRoutine = StartCoroutine(DetectLoop());
    }

    private void OnDisable()
    {
        if (detectRoutine != null)
            StopCoroutine(detectRoutine);
        detectRoutine = null;
    }

    private IEnumerator DetectLoop()
    {
        var wait = new WaitForSeconds(detectInterval);

        while (true)
        {
            DetectPlayer();
            yield return wait;
        }
    }

    private void DetectPlayer()
    {
        IsPlayerVisible = false;

        var player = Player.Instance;
        if (player == null || player.IsHidden) return;
        if (rayPoint == null || enemy == null) return;

        Vector2 origin = rayPoint.position;
        Vector2 dir = new Vector2(enemy.Direction, 0f);

        float distToPlayer = Mathf.Abs(player.transform.position.x - origin.x);
        if (distToPlayer > viewDistance) return;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, combinedMask);
        if (hit.collider == null) return;

        if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
        {
            if (hit.collider.CompareTag(TagName.Player))
                IsPlayerVisible = true;
        }

        Debug.DrawRay(origin, dir * viewDistance, IsPlayerVisible ? Color.red : Color.green);
    }
}
