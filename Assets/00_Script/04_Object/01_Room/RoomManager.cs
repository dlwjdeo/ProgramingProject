using System.Collections.Generic;
using UnityEngine;

//현재 플레이어가 있는 방의 위치를 조정해주는 Manager
public class RoomManager : Singleton<RoomManager>
{

    [Header("방 정보")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    [SerializeField] private List<Portal> portals = new List<Portal>();
    [SerializeField] private RoomController playerRoom;
    [SerializeField] private RoomController enemyRoom;
    
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Collider2D enemyCollider;
    


    private void Start()
    {
        // 시작 시 현재 방이 지정되어 있지 않다면 플레이어 중심 기준으로 자동 탐색
        if (playerRoom == null && playerCollider != null)
        {
            updatePlayerRoom(playerCollider.bounds.center);
        }
        else if (playerRoom != null)
        {
            playerRoom.Activate();
        }

        if (enemyRoom == null && enemyCollider != null)
        {
            updateEnemyRoom(enemyCollider.bounds.center);
        }
    }

    private void Update()
    {
        if (playerCollider != null)
        {
            updatePlayerRoom(playerCollider.bounds.center);
        }
        if(enemyRoom != null)
        {
            updateEnemyRoom(enemyCollider.bounds.center);
        }
    }
    public Portal FindClosestPortal(int fromFloor, int toFloor, Vector3 enemyPos)
    {
        List<Portal> candidates = portals.FindAll(p => p.FromFloor == fromFloor && p.ToFloor == toFloor);
        if (candidates.Count == 0) return null;

        float minDist = float.MaxValue;
        Portal closest = null;

        foreach (var portal in candidates)
        {
            float dist = Vector2.Distance(enemyPos, portal.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = portal;
            }
        }

        return closest;
    }
    private void updatePlayerRoom(Vector2 point)
    {
        foreach (var room in rooms)
        {
            if (room == null) continue;
            if (room.Collider2D.OverlapPoint(point))
            {
                setPlayerRoom(room);
                break;
            }
        }
    }

    private void updateEnemyRoom(Vector2 point)
    {
        foreach (var room in rooms)
        {
            if (room == null) continue;
            if (room.Collider2D.OverlapPoint(point))
            {
                enemyRoom = room;
                break;
            }
        }
    }

    private void setPlayerRoom(RoomController newRoom)
    {
        if (newRoom == null || newRoom == playerRoom) return;

        if (playerRoom != null) playerRoom.Deactivate();

        playerRoom = newRoom;
        playerRoom.Activate();
    }

    public RoomController GetPlayerRoom()
    {
        return playerRoom;
    }

    public RoomController GetEnemyRoom()
    {
        return enemyRoom;
    }

    public bool IsSameFloor()
    {
        return playerRoom.Floor == enemyRoom.Floor;
    }
}