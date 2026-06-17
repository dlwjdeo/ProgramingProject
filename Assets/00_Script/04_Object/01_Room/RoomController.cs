using UnityEngine;

//���ο� �ִ� overlay�� �״ٰ� �����ִ� class
public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private RoomEntranceTrigger leftEntrance;
    [SerializeField] private RoomEntranceTrigger rightEntrance;
    [SerializeField] private bool isOpened;

    [Header("Room Info")]
    [SerializeField] private int floor;
    public int Floor => floor;

    public Collider2D Collider2D => _collider2D;

    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (_collider2D == null) _collider2D = GetComponentInChildren<Collider2D>();
        Deactivate();
    }

    public void Activate()
    {
        if (overlay != null) overlay.SetActive(false);
    }

    public void Deactivate()
    {
        if (overlay != null) overlay.SetActive(true);
    }

    public Vector3 GetCenterPosition()
    {
         return _collider2D.bounds.center;
    }

    /// <summary>
    /// 특정 월드 좌표가 이 방 안에 있는지 확인합니다.
    /// </summary>
    public bool ContainsPoint(Vector2 worldPoint)
    {
        if (_collider2D == null) return false;
        return _collider2D.OverlapPoint(worldPoint);
    }

    /// <summary>
    /// 플레이어가 이 방 안에 있는지 확인합니다.
    /// </summary>
    public bool IsPlayerInRoom()
    {
        Player player = Player.Instance;
        if (player == null || _collider2D == null) return false;

        // 플레이어의 콜라이더 중심 위치 또는 트랜스폼 위치 사용
        Collider2D playerCollider = player._Collider2D != null 
            ? player._Collider2D 
            : player.GetComponent<Collider2D>();

        Vector2 playerPos = playerCollider != null
            ? playerCollider.bounds.center
            : (Vector2)player.transform.position;

        return ContainsPoint(playerPos);
    }

    /// <summary>
    /// 특정 콜라이더가 이 방 안에 있는지 확인합니다.
    /// </summary>
    public bool IsColliderInRoom(Collider2D collider)
    {
        if (collider == null || _collider2D == null) return false;

        Vector2 colliderPos = collider.bounds.center;
        return ContainsPoint(colliderPos);
    }
}