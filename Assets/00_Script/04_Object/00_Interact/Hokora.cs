using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hokora : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite bowlSprite;
    [SerializeField] private Sprite sultSprite;
    [SerializeField] private Sprite allSprite;
    [SerializeField] private Door door;
    private bool isBowlPlaced = false;
    private bool isSaltPlaced = false;
    public override void Interact()
    {
        if(player == null) return;
        if(player.PlayerInventory.CurrentItem == Item.OfferingBowl)
        {
            isBowlPlaced = true;
            if(!isSaltPlaced)
            {
                spriteRenderer.sprite = bowlSprite;
            }
            else
            {
                spriteRenderer.sprite = allSprite;
                door.Unlock();
                ShowSuccess();
            }
            player.PlayerInventory.ClearItem();
            SoundManager.Instance?.PlayItemPickUpCue();
            return;
        }
        else if(player.PlayerInventory.CurrentItem == Item.Salt && !isSaltPlaced && !isBowlPlaced)
        {
            isSaltPlaced = true;
            if(!isBowlPlaced)
            {
                spriteRenderer.sprite = sultSprite;
            }
            else
            {
                spriteRenderer.sprite = allSprite;
                door.Unlock();
                ShowSuccess();
            }
            player.PlayerInventory.ClearItem();
            SoundManager.Instance?.PlayItemPickUpCue();
            return;
        }
        if(isBowlPlaced && isSaltPlaced && player.PlayerInventory.CurrentItem == Item.Hair)
        {
            spriteRenderer.sprite = allSprite;
            StartCoroutine(EnemyDieCoroutine());
            return;
        }
    }

    private IEnumerator EnemyDieCoroutine()
    {
        Enemy enemy = FindObjectOfType<Enemy>();
        enemy.StateMachine.ChangeState(enemy.StateMachine.Die);
        
        yield return new WaitForSeconds(1f);
        SetPriority(0);

        

        yield return new WaitForSeconds(2f);

        StartCoroutine(UIManager.Instance.FadeOut());
    }
}
