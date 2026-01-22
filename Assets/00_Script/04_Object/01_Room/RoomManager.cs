using System;
using System.Collections.Generic;
using UnityEngine;
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public event Action<RoomController> OnChangedPlayerRoom;
    public event Action<RoomController> OnChangedEnemyRoom;

    [Header("방 정보")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    public IReadOnlyList<RoomController> Rooms => rooms;

    [SerializeField] private List<Portal> portals = new List<Portal>();

    [Header("초기 할당 인덱스")]
    [SerializeField] private int playerRoomIndex = 0;
    [SerializeField] private int enemyRoomIndex = 3;

    public RoomController PlayerRoom { get; private set; }
    public RoomController EnemyRoom { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeRoomsSafe();
    }

    private void InitializeRoomsSafe()
    {
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("[RoomManager] rooms list is empty.");
            return;
        }

        playerRoomIndex = Mathf.Clamp(playerRoomIndex, 0, rooms.Count - 1);
        enemyRoomIndex = Mathf.Clamp(enemyRoomIndex, 0, rooms.Count - 1);

        SetPlayerRoom(rooms[playerRoomIndex]);
        SetEnemyRoom(rooms[enemyRoomIndex]);
    }

    public void SetPlayerRoom(RoomController room)
    {
        if (room == null) return;
        if (PlayerRoom == room) return;

        var prev = PlayerRoom;
        PlayerRoom = room;

        if (prev != null)
            prev.Deactivate();

        PlayerRoom.Activate();
        OnChangedPlayerRoom?.Invoke(PlayerRoom);
    }

    public void SetEnemyRoom(RoomController room)
    {
        if (room == null) return;
        if (EnemyRoom == room) return;

        EnemyRoom = room;
        OnChangedEnemyRoom?.Invoke(EnemyRoom);
    }

    public Portal FindClosestPortal(int fromFloor, int toFloor, Vector3 enemyPos)
    {
        if (portals == null || portals.Count == 0) return null;

        if (fromFloor < toFloor) toFloor = fromFloor + 1;
        else if (fromFloor > toFloor) toFloor = fromFloor - 1;
        else return null; 

        Portal closest = null;
        float minSqrDist = float.MaxValue;

        for (int i = 0; i < portals.Count; i++)
        {
            Portal p = portals[i];
            if (p == null) continue;

            if (!p.IsOpened) continue;
            if (p.FromFloor != fromFloor) continue;
            if (p.ToFloor != toFloor) continue;

            float sqrDist = (p.transform.position - enemyPos).sqrMagnitude;
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                closest = p;
            }
        }

        return closest;
    }

    public RoomController GetRandomRoom(RoomController currentRoom)
    {
        if (rooms == null || rooms.Count == 0) return null;
        while(true)
        {
            int index = UnityEngine.Random.Range(0, rooms.Count);
            if(rooms[index] == currentRoom) // 현재 방이면 다시 선택
                continue;

            if (rooms[index].IsOpened)
            {
                Debug.Log($"[RoomManager] GetRandomRoom: {index}");
                return rooms[index];
            }
        }
    }
}
