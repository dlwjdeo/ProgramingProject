using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("Destination")]
    [SerializeField] private Portal targetPortal;
    [SerializeField] private CameraArea targetArea;
    [SerializeField] private RoomController targetRoom;

    [Header("Floor Info")]
    [SerializeField] private int fromFloor;
    [SerializeField] private int toFloor;

    [Header("Lock")]
    [SerializeField] private Item keyItem;
    [SerializeField] private bool isOpened = false;
    public bool IsOpened => isOpened;
    public int FromFloor => fromFloor;
    public int ToFloor => toFloor;

    private readonly HashSet<int> _operatingEnemies = new HashSet<int>();

    public override void Interact()
    {
        if (!isOpened)
        {
            TryOpen();
            return;
        }

        if (player != null)
        {
            InteractPortal(player.transform, isPlayer: true);
        }
    }

    private void TryOpen()
    {
        if (Player.Instance == null || Player.Instance.PlayerInventory == null)
        {
            ShowFail();
            return;
        }

        if (keyItem == Player.Instance.PlayerInventory.CurrentItem)
        {
            OpenPortal();
            if (targetPortal != null) targetPortal.OpenPortal(); // ½Ö¹æÇâ ¿ÀÇÂ
            ShowSuccess();
        }
        else
        {
            ShowFail();
        }
    }

    public void InteractPortal(Transform target, bool isPlayer)
    {
        if (target == null) return;
        if (!isOpened) return;
        if (targetPortal == null) return;

        if (isPlayer)
        {
            Player p = target.GetComponent<Player>();
            if (p != null)
                p.StartCoroutine(MoveRoomForPlayer(p));
        }
        else
        {
            Enemy e = target.GetComponent<Enemy>();
            if (e == null) return;

            int id = e.GetInstanceID();
            if (_operatingEnemies.Contains(id)) return;

            e.StartCoroutine(MoveRoomForEnemy(e));
        }
    }

    private IEnumerator MoveRoomForPlayer(Player p)
    {
        if (p.PlayerMover != null)
        {
            p.PlayerMover.SetMoveEnabled(false);
            p.PlayerMover.SetMoveInput(Vector2.zero);
        }

        if (UIManager.Instance != null)
            yield return UIManager.Instance.FadeOut();

        p.transform.position = targetPortal.transform.position;

        if (RoomManager.Instance != null)
            RoomManager.Instance.SetPlayerRoom(targetRoom);

        if (CameraManager.Instance != null)
            CameraManager.Instance.SwitchCamera(targetArea);

        if (UIManager.Instance != null)
            yield return UIManager.Instance.FadeIn();

        if (p.PlayerMover != null)
            p.PlayerMover.SetMoveEnabled(true);
    }

    private IEnumerator MoveRoomForEnemy(Enemy e)
    {
        int id = e.GetInstanceID();
        _operatingEnemies.Add(id);

        e.SetMove(false);
        yield return new WaitForSeconds(1f);

        e.transform.position = targetPortal.transform.position;

        if (RoomManager.Instance != null)
            RoomManager.Instance.SetEnemyRoom(targetRoom);

        e.SetMove(true);

        _operatingEnemies.Remove(id);
    }

    public void OpenPortal()
    {
        isOpened = true;
    }
}
