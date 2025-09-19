using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("��Ż �̵� ��ǥ����")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private CameraArea targetArea;

    private PlayerMover playerMover;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // �÷��̾� ��ġ �̵�
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
        //�� �̵� �Է� �� ������ ����
        playerMover.SetMove(false);
        yield return fader.FadeOut();


        player.transform.position = targetPoint.position;
        CameraManager.Instance.SwitchCamera(nextArea);

        playerMover.SetMove(true);
        yield return fader.FadeIn();
    }
}
