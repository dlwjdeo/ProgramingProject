using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("��Ż �̵� ��ǥ����")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private CameraArea targetArea;
    [SerializeField] private int targetFloor;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // �÷��̾� ��ġ �̵�
            StartCoroutine(MoveRoom(targetArea));
        }
    }

    IEnumerator MoveRoom(CameraArea nextArea)
    {
        ScreenFader fader = FindObjectOfType<ScreenFader>();
        //�� �̵� �Է� �� ������ ����
        player.PlayerMover.SetMove(false);
        yield return fader.FadeOut();


        player.transform.position = targetPoint.position;
        CameraManager.Instance.SwitchCamera(nextArea);

        player.PlayerMover.SetMove(true);
        yield return fader.FadeIn();
    }
}
