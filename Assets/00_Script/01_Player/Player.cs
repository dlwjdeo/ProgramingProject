using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player> 
{
    [SerializeField] private PlayerStateType state;
    public PlayerStateType State => state;
    public Item Item = Item.Null;

    public event Action<Item> OnItemChanged;

    public PlayerInputReader PlayerInputReader {  get; private set; }
    public PlayerMover PlayerMover { get; private set; }
    public PlayerInteraction PlayerInteraction { get; private set; }

    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public Rigidbody2D Rigidbody2D { get; private set; }
    public BoxCollider2D _Collider2D { get; private set; }
    public float LastHideTime { get; private set; }
    public bool IsHidden { get; private set; }

    public RoomController CurrentRoom { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        PlayerInputReader = GetComponent<PlayerInputReader>();
        PlayerMover = GetComponent<PlayerMover>();
        PlayerInteraction = GetComponent<PlayerInteraction>();
        PlayerStateMachine = new PlayerStateMachine(this);
        Rigidbody2D = GetComponent<Rigidbody2D>();
        _Collider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        UpdateCurrentRoom();
        PlayerStateMachine.Update();
    }

    private void UpdateCurrentRoom()
    {
        Vector2 center = _Collider2D.bounds.center;

        foreach (var room in RoomManager.Instance.Rooms)
        {
            if (room == null) continue;

            if (room.Collider2D.OverlapPoint(center))
            {
                if (room != CurrentRoom)
                {
                    CurrentRoom?.Deactivate();
                    CurrentRoom = room;
                    CurrentRoom.Activate();
                }
                return;
            }
        }
    }
    public void ChangeItem(Item changeItem)
    {
        Item = changeItem;
        OnItemChanged?.Invoke(Item);
    }
    public void SetStateType(PlayerStateType type) {  state = type; }

    public void SetHidden(bool hidden) 
    {
        IsHidden = hidden; 
        if(IsHidden == true)
        {
            LastHideTime = Time.time;
        }
    }
}
