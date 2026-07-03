using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll : Interactable
{
    [SerializeField] private Item keyItem = Item.Match;
    public override void Interact()
    {
        if (player == null || player.PlayerInventory.CurrentItem != keyItem) return;
        player.PlayerInventory.ClearItem();
    }
}
