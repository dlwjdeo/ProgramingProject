using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("포탈 이동 목표지점")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private CameraArea targetArea;

    [SerializeField] private int fromFloor;
    [SerializeField] private int toFloor;
    public int FromFloor => fromFloor;
    public int ToFloor => toFloor;
    public Transform TargetPoint => targetPoint;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // 플레이어 위치 이동
            InteractPortal(player.transform, true);
        }
    }

    public void InteractPortal(Transform target, bool isPlayer)
    {
        if (target == null || targetPoint == null) return;

        if (isPlayer)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
                player.StartCoroutine(MoveRoomForPlayer(player));
        }
        else
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
                enemy.StartCoroutine(MoveRoomForEnemy(enemy));
        }
    }

    private IEnumerator MoveRoomForPlayer(Player player)
    {
        ScreenFader fader = FindObjectOfType<ScreenFader>();

        player.PlayerMover.SetMove(false);
        yield return fader.FadeOut();

        player.transform.position = targetPoint.position;
        CameraManager.Instance.SwitchCamera(targetArea);

        player.PlayerMover.SetMove(true);
        yield return fader.FadeIn();
    }

    private IEnumerator MoveRoomForEnemy(Enemy enemy)
    {
        enemy.SetMove(false);
        yield return new WaitForSeconds(0.1f);

        enemy.transform.position = targetPoint.position;
        enemy.SetMove(true);
    }
}
