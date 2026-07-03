using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Portal portal;
    public override void Interact()
    {
        if (player == null || player.PlayerInventory.CurrentItem != keyItem)
        {
            ShowFail();
            return;
        }
        player.PlayerInventory.ClearItem();
        if (portal != null)
        {
            portal.OpenPortal();
            GetComponent<SpriteRenderer>().enabled = false;
            SetPriority(0);
        }
    }
}
