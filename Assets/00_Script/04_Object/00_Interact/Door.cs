using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private Item keyItem;

    [SerializeField] private Collider2D doorCollider;

    public override void Interact()
    {
        if (isLocked)
        {
            if (Player.Instance.PlayerInventory.HasItem(keyItem))
            {
                unlock();
                ShowSuccess();
            }
            else
            {
                ShowFail();
            }
        }
        else
        {
            Open();
        }
    }

    private void unlock()
    {
        isLocked = false;
        doorCollider.isTrigger = true;
    }

    private void Open()
    {
        // 포탈로 이동 or 애니메이션 등
    }
}
