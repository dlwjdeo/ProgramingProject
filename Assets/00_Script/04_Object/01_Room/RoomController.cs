using UnityEngine;

//내부에 있는 overlay만 켰다가 지워주는 class
public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private Collider2D _collider2D;

    [Header("방 정보")]
    [SerializeField] private int floor;
    public int Floor => floor;

    public Collider2D Collider2D => _collider2D;

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
}