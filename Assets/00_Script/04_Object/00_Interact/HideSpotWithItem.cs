using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpotWithItem : Interactable
{
    [SerializeField] private Item item;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite interactSprite;
    private bool isUsed = false;

    public override void Interact()
    {
        if (player == null) return;

        if (!isUsed)
        {
            if (!player.PlayerInventory.TryAddItem(item))
            {
                ShowFail();
                return;
            }

            isUsed = true;
            spriteRenderer.sprite = interactSprite;
            GameObject itemObject = Instantiate(itemPrefab, transform.position + Vector3.down * 0.5f, Quaternion.identity);
            itemObject.SetActive(false);
            player.PlayerInventory.SetItemObject(itemObject.GetComponent<PickupItem>());
            ShowSuccess();
            return;
        }

        if (player.State != PlayerStateType.Hide)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Hide);
        }
    }
}
