using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("포탈 이동 목표지점")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private CameraArea targetArea;

    private PlayerMover playerMover;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // 플레이어 위치 이동
            StartCoroutine(MoveRoom(targetArea));
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.TryGetComponent<PlayerMover>(out var player))
        {
            playerMover = player;
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.TryGetComponent<PlayerMover>(out var player))
        {
            playerMover = null;
        }
    }

    IEnumerator MoveRoom(CameraArea nextArea)
    {
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        //방 이동 입력 시 움직임 제어
        playerMover.SetMove(false);
        yield return fader.FadeOut();


        player.transform.position = targetPoint.position;
        CameraManager.Instance.SwitchCamera(nextArea);

        playerMover.SetMove(true);
        yield return fader.FadeIn();
    }
}
