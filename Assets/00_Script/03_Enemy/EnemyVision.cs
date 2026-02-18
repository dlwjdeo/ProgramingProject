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

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, viewDistance, combinedMask);
        if (hit.collider == null) return;

        if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
        {
            if (hit.collider.CompareTag(TagName.Player))
                IsPlayerVisible = true;
        }
        else
        {
            var door = hit.collider.GetComponentInParent<Door>();
            if (door != null)
            {
                if (hit.distance <= doorOpenDistance && !door.IsOpen && !door.IsLocked)
                    PendingDoorToOpen = door;
            }
        }

        Debug.DrawRay(origin, dir * viewDistance, IsPlayerVisible ? Color.red : Color.green);
    }

    public bool TryGetDoorToOpen(out Door door)
    {
        door = PendingDoorToOpen;
        return door != null && !door.IsOpen && !door.IsLocked;
    }
}
