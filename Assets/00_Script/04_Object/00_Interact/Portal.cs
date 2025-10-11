using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("��Ż �̵� ��ǥ����")]
    [SerializeField] private Portal targetPortal;
    [SerializeField] private CameraArea targetArea;

    [SerializeField] private int fromFloor;
    [SerializeField] private int toFloor;

    [Header("���")]
    [SerializeField] private Item keyItem;
    [SerializeField] private bool isLocked;
    public int FromFloor => fromFloor;
    public int ToFloor => toFloor;
    private bool isOperated = false;

    public override void Interact()
    {
        if (isLocked)
        {
            if (keyItem == Player.Instance.Item)
            {
                OpenPortal();
                targetPortal.OpenPortal(); //�ֹ��� ��Ż ����
                ShowSuccess();
            }
            else
            {
                ShowFail();
            }
        }
        if (player != null && targetPortal != null && !isLocked)
        {
            // �÷��̾� ��ġ �̵�
            InteractPortal(player.transform, true);
        }
    }

    public void InteractPortal(Transform target, bool isPlayer)
    {
        if (target == null || targetPortal == null) return;

        if (isPlayer)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
                player.StartCoroutine(MoveRoomForPlayer(player));
        }
        else
        {
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null && isOperated == false)
                enemy.StartCoroutine(MoveRoomForEnemy(enemy));
        }
    }

    private IEnumerator MoveRoomForPlayer(Player player)
    {
        player.PlayerMover.SetMove(false);
        yield return UIManager.Instance.FadeOut();

        player.transform.position = targetPortal.transform.position;
        CameraManager.Instance.SwitchCamera(targetArea);

        player.PlayerMover.SetMove(true);
        yield return UIManager.Instance.FadeIn();
    }

    private IEnumerator MoveRoomForEnemy(Enemy enemy)
    {
        isOperated = true;
        enemy.SetMove(false);
        yield return new WaitForSeconds(1f);

        enemy.transform.position = targetPortal.transform.position;
        enemy.SetMove(true);
        isOperated = false;
    }

    public void OpenPortal()
    {
        isLocked = false;
    }
}
