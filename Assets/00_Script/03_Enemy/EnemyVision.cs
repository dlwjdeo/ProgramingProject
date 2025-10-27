using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [Header("시야 설정")]
    [SerializeField] private Transform rayPoint;
    [SerializeField] private float viewDistance = 5f;
    [SerializeField] private float detectInterval = 0.1f; // 감지 주기 (0.1초마다 검사)
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;
    public bool IsPlayerVisible { get; private set; }

    private Enemy enemy;
    private Coroutine detectRoutine;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnEnable()
    {
        detectRoutine = StartCoroutine(detectLoop());
    }

    private void OnDisable()
    {
        if (detectRoutine != null)
            StopCoroutine(detectRoutine);
    }

    private IEnumerator detectLoop()
    {
        var wait = new WaitForSeconds(detectInterval);

        while (true)
        {
            detectPlayer();
            yield return wait;
        }
    }

    private void detectPlayer()
    {
        IsPlayerVisible = false;

        var player = Player.Instance;
        if (player == null || player.IsHidden)
            return;

        Vector2 origin = rayPoint.position;
        Vector2 lookDir = new Vector2(enemy.Direction, 0f);
        float dist = Vector2.Distance(origin, player.transform.position);

        if (Mathf.Abs(dist) > viewDistance)
            return;

        RaycastHit2D hit = Physics2D.Raycast(origin, lookDir, viewDistance, playerMask);
        if (hit.collider != null && hit.collider.CompareTag(TagName.Player))
        {
            IsPlayerVisible = true;
        }

        Debug.DrawRay(origin, lookDir * viewDistance, IsPlayerVisible ? Color.red : Color.green);
    }
}
