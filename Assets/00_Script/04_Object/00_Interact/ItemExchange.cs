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
            player.PlayerInventory.DropItem();
            GameObject bowl = Instantiate(offeringBowl, transform.position + Vector3.down * 0.5f, Quaternion.identity);
            bowl.SetActive(false);

            if (!player.PlayerInventory.TryAddItem(bowl.GetComponent<PickupItem>().ItemData))
            {
                Debug.LogWarning("인벤토리가 비어있지 않아 공양 그릇을 획득하지 못했습니다.");
                Destroy(bowl);
                isInteracted = false;
                spriteRenderer.sprite = normalSprite;
                ShowFail();
                return;
            }

            player.PlayerInventory.SetItemObject(bowl.GetComponent<PickupItem>());
            ShowSuccess();
        }
        else
        {
            ShowFail();
        }
    }
}
