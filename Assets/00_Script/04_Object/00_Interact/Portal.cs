using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("Visual")]
    [SerializeField] private SpriteRenderer portalRenderer;
    [SerializeField, Min(0.01f)] private float visibleXRange = 3f;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;

    [Header("Destination")]
    [SerializeField] private Portal targetPortal;
    [SerializeField] private CameraArea targetArea;
    [SerializeField] private RoomController targetRoom;
    public Transform portalPosition;

    [Header("Floor Info")]
    [SerializeField] private int fromFloor;
    [SerializeField] private int toFloor;

    [Header("Lock")]
    [SerializeField] private Item keyItem;
    [SerializeField] private bool isOpened = false;

    [SerializeField] private bool isDownwardPortal = false;

    
    public bool IsOpened => isOpened;
    public int FromFloor => fromFloor;
    public int ToFloor => toFloor;

    private readonly HashSet<int> _operatingEnemies = new HashSet<int>();

    protected override void Awake()
    {
        base.Awake();

        if (portalRenderer == null)
            portalRenderer = GetComponent<SpriteRenderer>();

        if(!isDownwardPortal) return;
        ApplyAlpha(minAlpha);
    }

    private void Update()
    {
        if(!isDownwardPortal) return;
        UpdateXAxisAlpha();
    }

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
        if (Player.Instance == null)
        {
            ShowFail();
            return;
        }

        if (keyItem == Player.Instance.PlayerInventory.CurrentItem)
        {
            OpenPortal();
            if (targetPortal != null) targetPortal.OpenPortal(); // ????? ????
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

        // ?? ?? ? ? ??? ??
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayDoorOpenAt(transform.position);

        if (UIManager.Instance != null)
            yield return UIManager.Instance.FadeOut();

        if(targetPortal != null && targetPortal.portalPosition != null)
        {
            p.transform.position = targetPortal.portalPosition.position;
        }
        else
        {
            p.transform.position = targetPortal.transform.position;
        }

        if (RoomManager.Instance != null)
            RoomManager.Instance.SetPlayerRoom(targetRoom);

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

        // ?? ?? ? ? ??? ??
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayDoorOpenAt(transform.position, true);

        if (targetPortal != null && targetPortal.portalPosition != null)
            e.transform.position = targetPortal.portalPosition.position;
        else
            e.transform.position = targetPortal.transform.position;

        if (RoomManager.Instance != null)
            RoomManager.Instance.SetEnemyRoom(targetRoom);

        e.SetMove(true);

        _operatingEnemies.Remove(id);
    }

    public void OpenPortal()
    {
        isOpened = true;
        
        // ?? ??? ?? (?? ?? ??)
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayKeyUseCue();
    }

    public override void SetInteractable()
    {
        if(!isDownwardPortal)
        {
            base.SetInteractable();
            return;
        }
        UpdateXAxisAlpha();
    }

    public override void SetDefault()
    {
        if(!isDownwardPortal)
        {
            base.SetDefault();
            return;
        }
        UpdateXAxisAlpha();
    }

    private void UpdateXAxisAlpha()
    {
        if (portalRenderer == null)
            return;

        Transform playerTransform = Player.Instance != null ? Player.Instance.transform : null;
        if (playerTransform == null)
        {
            ApplyAlpha(minAlpha);
            return;
        }

        float xDiff = Mathf.Abs(playerTransform.position.x - transform.position.x);
        float t = Mathf.Clamp01(1f - (xDiff / visibleXRange));
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);
        ApplyAlpha(alpha);
    }

    private void ApplyAlpha(float alpha)
    {
        if (portalRenderer == null)
            return;

        Color color = portalRenderer.color;
        color.a = alpha;
        portalRenderer.color = color;
    }
}
