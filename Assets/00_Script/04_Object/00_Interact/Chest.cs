using System;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Item dropItem;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Transform dropPoint;

    private bool isOpened = false;
    public override void Interact()
    {
        if (isOpened) return;

        if (player == null) return;

        if (keyItem == Item.Null || player.PlayerInventory.CurrentItem == keyItem)
        {
            OpenChest();
        }
        else
        {
            ShowFail();
        }
    }
    private void OpenChest()
    {
        isOpened = true;
        SetPriority(0);
        ShowSuccess();

        if (dropPrefab != null && dropPoint != null)
        {
            Instantiate(dropPrefab, dropPoint.position, Quaternion.identity);
        }
    }
}
