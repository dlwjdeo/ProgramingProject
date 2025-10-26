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

    public void DropItem()
    {
        if (itemObject == null) return;

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
}
