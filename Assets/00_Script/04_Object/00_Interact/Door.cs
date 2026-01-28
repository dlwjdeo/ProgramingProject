using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private bool isLocked = true;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Item keyItem;

    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private bool canInteractWhenUnlocked = true;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Interact()
    {
        if (isLocked)
        {
            if (Player.Instance.PlayerInventory.HasItem(keyItem))
            {
                Unlock();
                ShowSuccess();
            }
            else
            {
                ShowFail();
            }
            return;
        }
        if( !canInteractWhenUnlocked ) return;
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Open()
    {
        isOpen = true;

        doorCollider.isTrigger = true;
        spriteRenderer.color = Color.green;
    }

    public void Close()
    {
        isOpen = false;

        doorCollider.isTrigger = false;
        spriteRenderer.color = Color.white;
        Debug.Log("Door Closed");
    }
}
