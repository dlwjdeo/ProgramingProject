using System.Collections.Generic;
using UnityEngine;

//현재 플레이어가 있는 방의 위치를 조정해주는 Manager
public class RoomManager : Singleton<RoomManager>
{

    [Header("방 정보")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    [SerializeField] private RoomController currentRoom;
    
    [SerializeField] private Collider2D playerCollider; 


    private void Start()
    {
        // 시작 시 현재 방이 지정되어 있지 않다면 플레이어 중심 기준으로 자동 탐색
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