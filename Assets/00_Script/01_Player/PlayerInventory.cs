using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Item currentItem;
    public Item CurrentItem => currentItem;
    private PickupItem itemObject;
    public event Action<Item> OnItemChanged;
    private PlayerInputReader inputReader;

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
    }
    private void OnEnable()
    {
        inputReader.DropItem += DropItem;
    }

    private void OnDisable()
    {
        inputReader.DropItem -= DropItem;
    }
    public void ChangeItem(Item changeItem)
    {
        currentItem = changeItem;
        OnItemChanged?.Invoke(currentItem);
    }

    public bool TryAddItem(Item item)
    {
        if (currentItem != Item.Null) return false;
        ChangeItem(item);
        
        // 아이템 주울 때 소리
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayItemPickUpCue();
        
        return true;
    }

    public void DropItem()
    {
        if (itemObject == null) return;

        // 아이템 떨어트릴 때 소리
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayItemDropCue();

        Vector3 dropPos = transform.position + Vector3.right * 0.5f;
        itemObject.transform.position = dropPos;
        itemObject.gameObject.SetActive(true);

        itemObject = null;
        currentItem = Item.Null;
        OnItemChanged?.Invoke(Item.Null);
    }

    public void SetItemObject(PickupItem pickupItem)
    {
        itemObject = pickupItem;
    }

    public bool HasItem(Item item) { return currentItem == item; }
    
    public void ClearItem()
    {
        currentItem = Item.Null;
        itemObject = null;
        OnItemChanged?.Invoke(Item.Null);
    }
}
