using System.Collections.Generic;
using UnityEngine;

//���� �÷��̾ �ִ� ���� ��ġ�� �������ִ� Manager
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("�� ����")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    [SerializeField] private RoomController currentRoom;
    
    [SerializeField] private Collider2D playerCollider; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
        }
        else 
        { 
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ���� �� ���� ���� �����Ǿ� ���� �ʴٸ� �÷��̾� �߽� �������� �ڵ� Ž��
        if (currentRoom == null && playerCollider != null)
        {
            UpdatePlayerRoom(playerCollider.bounds.center);
        }
        else if (currentRoom != null)
        {
            currentRoom.Activate();
        }
    }

    private void Update()
    {
        if (playerCollider != null)
        {
            UpdatePlayerRoom(playerCollider.bounds.center);
        }
    }

    public void UpdatePlayerRoom(Vector2 point)
    {
        foreach (var room in rooms)
        {
            if (room == null) continue;
            if (room.Collider2D.OverlapPoint(point))
            {
                SetCurrentRoom(room);
                break;
            }
        }
    }

    public void SetCurrentRoom(RoomController newRoom)
    {
        if (newRoom == null || newRoom == currentRoom) return;

        if (currentRoom != null) currentRoom.Deactivate();

        currentRoom = newRoom;
        currentRoom.Activate();
    }
}