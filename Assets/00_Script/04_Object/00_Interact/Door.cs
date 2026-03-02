using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    [SerializeField] private bool isLocked;
    [SerializeField] private bool isOpen;
    [SerializeField] private Item keyItem;

    [SerializeField] private Collider2D doorCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine openCoroutine;

    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;

    [SerializeField] private bool canInteractWhenUnlocked = true;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public override void Interact()
    {
        Debug.Log("문 상호작용");
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
            Open(0f);
        }
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Open(float delay = 0f)
    {
        if (isOpen) return;
        if (openCoroutine != null) return;

        openCoroutine = StartCoroutine(OpenCoroutine(delay));
    }

    private IEnumerator OpenCoroutine(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        isOpen = true;
        if (doorCollider != null)
            doorCollider.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.green;

        openCoroutine = null;
        Debug.Log("문 열림");
    }

    public void Close()
    {
        isOpen = false;

        doorCollider.enabled = true;
        Debug.Log(doorCollider.isTrigger);
        spriteRenderer.color = Color.red;
    }
}
