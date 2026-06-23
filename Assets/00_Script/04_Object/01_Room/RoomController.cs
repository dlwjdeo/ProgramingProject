using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private bool isOpened;

    [Header("Room Info")]
    [SerializeField] private int floor;
    public int Floor => floor;

    public Collider2D Collider2D => _collider2D;

    public bool IsOpened => isOpened;

    private bool isPlayerDetected = false;
    private bool isEnemyDetected = false;

    private RoomManager roomManager;

    private void Awake()
    {
        if (_collider2D == null) _collider2D = GetComponentInChildren<Collider2D>();
        roomManager = GetComponentInParent<RoomManager>();
        Deactivate();
    }

    private void Update()
    {
        bool isPlayerInRoom = _collider2D.OverlapPoint(roomManager.Player.transform.position);
        roomManager.SetPlayerRoom(isPlayerInRoom ? this : null);
        
        bool isEnemyInRoom = _collider2D.OverlapPoint(roomManager.Enemy.transform.position);
        roomManager.SetEnemyRoom(isEnemyInRoom ? this : null);
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
    public bool ContainsPoint(Vector2 worldPoint)
    {
        if (_collider2D == null) return false;
        return _collider2D.OverlapPoint(worldPoint);
    }

}