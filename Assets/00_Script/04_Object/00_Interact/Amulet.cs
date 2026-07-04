using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class Amulet : Interactable
{
    [SerializeField] private Item KeyItem;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private Sprite AmuletSprite;
    public override void Interact()
    {
        if (player == null) return;

        if(player.PlayerInventory.CurrentItem != KeyItem) return;

        player.PlayerInventory.ClearItem();
        GetComponent<SpriteRenderer>().sprite = AmuletSprite;

        StartCoroutine(StartTimeLine());
    }

    private IEnumerator StartTimeLine()
    {   
        GameManager.Instance.SetGameState(GameState.Ending);
        yield return new WaitForSeconds(1f);
        yield return UIManager.Instance.FadeOut();
        
        yield return new WaitForSeconds(0.3f);
        playableDirector.Play();

        yield return UIManager.Instance.FadeIn();
    }
}
