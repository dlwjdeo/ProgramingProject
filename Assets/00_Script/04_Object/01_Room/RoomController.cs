using UnityEngine;

//���ο� �ִ� overlay�� �״ٰ� �����ִ� class
public class RoomController : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private RoomEntranceTrigger leftEntrance;
    [SerializeField] private RoomEntranceTrigger rightEntrance;
    [SerializeField] private bool isOpened;

    [Header("�� ����")]
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

    public void SetOpen(bool isOpened) => this.isOpened = isOpened;
}