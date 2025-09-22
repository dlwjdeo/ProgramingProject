using System.Collections.Generic;
using UnityEngine;

//���� �÷��̾ �ִ� ���� ��ġ�� �������ִ� Manager
public class RoomManager : Singleton<RoomManager>
{

    [Header("�� ����")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    [SerializeField] private RoomController currentRoom;
    
    [SerializeField] private Collider2D playerCollider; 


    private void Start()
    {
        // ���� �� ���� ���� �����Ǿ� ���� �ʴٸ� �÷��̾� �߽� �������� �ڵ� Ž��
        if (currentRoom == null && playerCollider != null)
        {
            updatePlayerRoom(playerCollider.bounds.center);
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
            updatePlayerRoom(playerCollider.bounds.center);
        }
    }

    private void updatePlayerRoom(Vector2 point)
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

    public RoomController GetCurrentRoom()
    {
        return currentRoom;
    }
}