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
            StartCoroutine(MoveRoom(targetArea));
        }
    }

    IEnumerator MoveRoom(CameraArea nextArea)
    {
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        //방 이동 입력 시 움직임 제어
        player.PlayerMover.SetMove(false);
        yield return fader.FadeOut();


        player.transform.position = targetPoint.position;
        CameraManager.Instance.SwitchCamera(nextArea);

        player.PlayerMover.SetMove(true);
        yield return fader.FadeIn();
    }
}
