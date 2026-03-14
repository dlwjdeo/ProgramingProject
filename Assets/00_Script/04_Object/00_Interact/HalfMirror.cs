using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfMirror : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Door door;
    [SerializeField] private Sprite usedSprite;
    [SerializeField] private Sprite unusedSprite;

    private bool isUsed = false;

    public override void Interact()
    {
        if(isUsed) return;
        if(player == null) return;

        if(player.PlayerInventory.HasItem(keyItem))
        {
            door.Unlock();
            door.Open();
            isUsed = true;
            player.PlayerInventory.ClearItem();
            GetComponent<SpriteRenderer>().sprite = usedSprite;
            ShowSuccess();
        }
        else
        {
            ShowFail();
        }
    }
}
