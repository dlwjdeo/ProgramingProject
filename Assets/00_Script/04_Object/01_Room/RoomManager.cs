using System;
using System.Collections.Generic;
using UnityEngine;
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public event Action<RoomController> OnChangedPlayerRoom;
    public event Action<RoomController> OnChangedEnemyRoom;

    [Header("Room List")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    public IReadOnlyList<RoomController> Rooms => rooms;

    [SerializeField] private List<Portal> portals = new List<Portal>();

    [Header("Debug")]
    [SerializeField] private bool showAllRoomsForTest = true;

    [Header("Tracked Actors")]
    [SerializeField] private Player player;
    [SerializeField] private Enemy enemy;

    private Collider2D playerCollider;
    private Collider2D enemyCollider;

    public RoomController PlayerRoom;// { get; private set; }
    public RoomController EnemyRoom;// { get; private set; }

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
        CacheTrackedActors();
    }

    private void Update()
    {
        UpdateTrackedActorRooms();
    }

    private void CacheTrackedActors()
    {
        if (player == null)
            player = Player.Instance;

        if (enemy == null)
            enemy = FindObjectOfType<Enemy>();

        if (player != null && (playerCollider == null || playerCollider.gameObject != player.gameObject))
        {
            playerCollider = player._Collider2D != null
                ? player._Collider2D
                : player.GetComponent<Collider2D>();
        }

        if (enemy != null && (enemyCollider == null || enemyCollider.gameObject != enemy.gameObject))
            enemyCollider = enemy.GetComponent<Collider2D>();
    }

    private void UpdateTrackedActorRooms()
    {
        CacheTrackedActors();

        if (player != null)
        {
            Vector2 playerCenter = playerCollider != null
                ? playerCollider.bounds.center
                : (Vector2)player.transform.position;

            TryUpdatePlayerRoomByWorldPoint(playerCenter);
        }

        if (enemy != null)
        {
            Vector2 enemyCenter = enemyCollider != null
                ? enemyCollider.bounds.center
                : (Vector2)enemy.transform.position;

            TryUpdateEnemyRoomByWorldPoint(enemyCenter);
        }
    }

    public void SetPlayerRoom(RoomController room)
    {
        if (room == null) return;
        if (PlayerRoom == room) return;

        var prev = PlayerRoom;
        PlayerRoom = room;

        if (showAllRoomsForTest)
        {
            ActivateAllRooms();
        }
        else
        {
            if (prev != null)
                prev.Deactivate();

            PlayerRoom.Activate();
        }

        OnChangedPlayerRoom?.Invoke(PlayerRoom);
    }

    public void SetEnemyRoom(RoomController room)
    {
        if (room == null) return;
        if (EnemyRoom == room) return;

        EnemyRoom = room;
        OnChangedEnemyRoom?.Invoke(EnemyRoom);
    }

    public RoomController FindRoomByWorldPoint(Vector2 worldPoint, RoomController preferredRoom = null)
    {
        if (rooms == null || rooms.Count == 0) return null;

        if (preferredRoom != null && preferredRoom.Collider2D != null && preferredRoom.Collider2D.OverlapPoint(worldPoint))
            return preferredRoom;

        for (int i = 0; i < rooms.Count; i++)
        {
            RoomController room = rooms[i];
            if (room == null || room.Collider2D == null) continue;

            if (room.Collider2D.OverlapPoint(worldPoint))
                return room;
        }

        return null;
    }

    public bool TryUpdatePlayerRoomByWorldPoint(Vector2 worldPoint)
    {
        RoomController nextRoom = FindRoomByWorldPoint(worldPoint, PlayerRoom);
        if (nextRoom == null) return false;

        SetPlayerRoom(nextRoom);
        return true;
    }

    public bool TryUpdateEnemyRoomByWorldPoint(Vector2 worldPoint)
    {
        RoomController nextRoom = FindRoomByWorldPoint(worldPoint, EnemyRoom);
        if (nextRoom == null) return false;

        SetEnemyRoom(nextRoom);
        return true;
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

    public Portal FindClosestPortalByTargetX(int fromFloor, int toFloor, float targetX)
    {
        if (portals == null || portals.Count == 0) return null;

        if (fromFloor < toFloor) toFloor = fromFloor + 1;
        else if (fromFloor > toFloor) toFloor = fromFloor - 1;
        else return null;

        Portal closest = null;
        float minXDist = float.MaxValue;

        for (int i = 0; i < portals.Count; i++)
        {
            Portal p = portals[i];
            if (p == null) continue;

            if (!p.IsOpened) continue;
            if (p.FromFloor != fromFloor) continue;
            if (p.ToFloor != toFloor) continue;

            float xDist = Mathf.Abs(p.transform.position.x - targetX);
            if (xDist < minXDist)
            {
                minXDist = xDist;
                closest = p;
            }
        }

        return closest;
    }

    public RoomController GetRandomRoom(RoomController currentRoom)
    {
        if (rooms == null || rooms.Count == 0) return null;

        List<RoomController> candidates = new List<RoomController>();
        for (int i = 0; i < rooms.Count; i++)
        {
            RoomController room = rooms[i];
            if (room == null) continue;
            if (room == currentRoom) continue;
            if (!room.IsOpened) continue;

            candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            return currentRoom;
        }

        int randomIndex = UnityEngine.Random.Range(0, candidates.Count);
        RoomController selected = candidates[randomIndex];
        Debug.Log($"[RoomManager] GetRandomRoom: {selected.name}");
        return selected;
    }

    private void ActivateAllRooms()
    {
        if (rooms == null) return;

        for (int i = 0; i < rooms.Count; i++)
        {
            RoomController room = rooms[i];
            if (room == null) continue;
            room.Activate();
        }
    }
}
