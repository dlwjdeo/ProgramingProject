using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultItem : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private GameObject itemPrefab;
    public override void Interact()
    {
        if (player == null || player.PlayerInventory.CurrentItem != keyItem) return;
        player.PlayerInventory.ClearItem();
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}
