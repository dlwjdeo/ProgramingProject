using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemExchange : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite interactSprite;
    [SerializeField] private Item keyItem;
    [SerializeField] private GameObject offeringBowl;

    [SerializeField] private bool isInteracted = false;
    public override void Interact()
    {
        if(isInteracted) return;
        Debug.Log("아이템 교환 상호작용");
        if( player.PlayerInventory.CurrentItem == keyItem)
        {
            isInteracted = true;
            spriteRenderer.sprite = interactSprite;
            // 키 아이템은 교환 시 소비하고, 공양 그릇은 월드에 드롭한다.
            player.PlayerInventory.ClearItem();
            Instantiate(offeringBowl, transform.position + Vector3.down * 0.5f, Quaternion.identity);
            ShowSuccess();
        }
        else
        {
            ShowFail();
        }
    }
}
