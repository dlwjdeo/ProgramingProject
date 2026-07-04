using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class Hokora : Interactable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite bowlSprite;
    [SerializeField] private Sprite sultSprite;
    [SerializeField] private Sprite allSprite;
    [SerializeField] private Door door;
    [SerializeField] private PlayableDirector playableDirector;
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
        else if(player.PlayerInventory.CurrentItem == Item.Salt)
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
            player.PlayerInventory.ClearItem();
            SoundManager.Instance?.PlayItemPickUpCue();
            StartCoroutine(EnemyDieCoroutine());
            return;
        }
    }

    private IEnumerator EnemyDieCoroutine()
    {
        yield return new WaitForSeconds(1f);
        SetPriority(0);

        yield return UIManager.Instance.FadeOut();
        playableDirector.Play();


        StartCoroutine(UIManager.Instance.FadeIn());
    }
}
