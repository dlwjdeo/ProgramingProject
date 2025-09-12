using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player> 
{
    public Item Item = Item.Null;

    public event Action<Item> OnItemChanged;
    public void ChangeItem(Item changeItem)
    {
        Item = changeItem;
        OnItemChanged?.Invoke(Item);
    }

}
