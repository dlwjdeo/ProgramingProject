using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player> 
{
    public Item Item = Item.Null;

    public event Action<Item> OnItemChanged;

    public PlayerInputReader PlayerInputReader {  get; private set; }
    public PlayerMover PlayerMover { get; private set; }
    public PlayerInteraction PlayerInteraction { get; private set; }

    public PlayerStateMachine PlayerStateMachine { get; private set; }
    public Rigidbody2D Rigidbody2D { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        PlayerInputReader = GetComponent<PlayerInputReader>();
        PlayerMover = GetComponent<PlayerMover>();
        PlayerInteraction = GetComponent<PlayerInteraction>();
        PlayerStateMachine = new PlayerStateMachine(this);
        Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerStateMachine.Update();
    }
    public void ChangeItem(Item changeItem)
    {
        Item = changeItem;
        OnItemChanged?.Invoke(Item);
    }

}
