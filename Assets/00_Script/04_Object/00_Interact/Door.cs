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
    [SerializeField] private int[] openRoomIndices;
    private Coroutine openCoroutine;

    public bool IsLocked => isLocked;
    public bool IsOpen => isOpen;
    public bool IsOpening => openCoroutine != null;

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
            Open(0f);
        }
    }

    public void Unlock()
    {
        // 열쇠 사용 소리
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayKeyUseCue();
            
        isLocked = false;

        if(openRoomIndices == null || openRoomIndices.Length == 0) return;
        
        RoomManager.Instance.OpenRoom(openRoomIndices);
    }

    public bool Open(float delay = 0f, bool openedByEnemy = false)
    {
        if (isOpen) return false;
        if (openCoroutine != null) return false;

        openCoroutine = StartCoroutine(OpenCoroutine(delay, openedByEnemy));
        return true;
    }

    private IEnumerator OpenCoroutine(float delay, bool openedByEnemy)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        isOpen = true;
        if (doorCollider != null)
            doorCollider.enabled = false;
        if (spriteRenderer != null)
            //spriteRenderer.color = Color.green;

        // 문 열리는 소리
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayDoorOpenAt(transform.position, openedByEnemy);

        openCoroutine = null;
        Debug.Log("문 열림");
    }

    public void Close()
    {
        isOpen = false;

        doorCollider.enabled = true;
        Debug.Log(doorCollider.isTrigger);
        //spriteRenderer.color = Color.red;
    }

    public override void SetInteractable()
    {
        Debug.Log("문임");
    }
}
