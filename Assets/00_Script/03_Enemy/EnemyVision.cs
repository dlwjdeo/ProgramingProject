using System.Collections;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float doorOpenDistance = 1.5f;
    [SerializeField] private float detectInterval = 0.1f;

    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private LayerMask playerMask;

    public bool IsPlayerVisible { get; private set; }
    public Door PendingDoorToOpen { get; private set; }

    private Enemy enemy;
    private Coroutine detectRoutine;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
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
            DetectTargets();
            yield return wait;
        }
    }

    private void DetectTargets()
    {
        IsPlayerVisible = false;
        PendingDoorToOpen = null;

        var player = Player.Instance;
        if (player == null || player.IsHidden) return;
        if (rayPoint == null || enemy == null) return;

        Vector2 origin = rayPoint.position;
        Vector2 dir = new Vector2(enemy.Direction, 0f);

        // Player-only ray.
        RaycastHit2D playerHit = Physics2D.Raycast(origin, dir, viewDistance, playerMask);
        if (playerHit.collider != null && playerHit.collider.CompareTag(TagName.Player))
        {
            bool blockedByObstacle = IsObstacleBlockingBeforePlayer(origin, dir, playerHit.distance);
            IsPlayerVisible = !blockedByObstacle;
        }

        // Door-only ray: handles interactive door opening logic separately.
        RaycastHit2D doorHit = Physics2D.Raycast(origin, dir, doorOpenDistance, obstacleMask);
        if (doorHit.collider != null)
        {
            var door = doorHit.collider.GetComponentInParent<Door>();
            if (door != null && !door.IsOpen && !door.IsLocked && !door.IsOpening)
            {
                PendingDoorToOpen = door;
            }
        }

        Debug.DrawRay(origin, dir * viewDistance, IsPlayerVisible ? Color.red : Color.green);
        Debug.DrawRay(origin, dir * doorOpenDistance, PendingDoorToOpen != null ? Color.yellow : Color.cyan);
    }

    public bool TryGetDoorToOpen(out Door door)
    {
        door = PendingDoorToOpen;
        return door != null && !door.IsOpen && !door.IsLocked && !door.IsOpening;
    }

    private bool IsObstacleBlockingBeforePlayer(Vector2 origin, Vector2 dir, float playerDistance)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, playerDistance, obstacleMask);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider == null) continue;
            if (hit.distance <= 0f) continue;

            Door door = hit.collider.GetComponentInParent<Door>();
            if (door != null)
            {
                if (door.IsOpen || door.IsOpening)
                {
                    continue;
                }

                return true;
            }

            return true;
        }

        return false;
    }
}
