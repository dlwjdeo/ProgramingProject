using System;
using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Item dropItem;
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private Sprite closedSprite;

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
        GetComponent<SpriteRenderer>().sprite = openedSprite;

        if (dropPrefab != null && dropPoint != null)
        {
            Instantiate(dropPrefab, dropPoint.position, Quaternion.identity);
        }
    }
}
