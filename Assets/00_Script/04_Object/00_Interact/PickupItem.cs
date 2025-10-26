using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PickupItem : Interactable
{
    [SerializeField] private Item item;
    public override void Interact()
    {
        if (player == null) return;
        if (player.PlayerInventory.CurrentItem != Item.Null) return;

        player.PlayerInventory.ChangeItem(item);
        player.PlayerInventory.SetItemObject(this);

        ShowSuccess();
        gameObject.SetActive(false);
    }
}
