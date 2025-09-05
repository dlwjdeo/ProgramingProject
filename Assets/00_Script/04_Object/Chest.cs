using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private Item KeyItem;
    public Player player;
    public void Interact()
    {
        Debug.Log("상자 작동");
        if(player != null && player.item == KeyItem)
        {
            Debug.Log("열렸습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagName.Player))
        {
            player = collision.GetComponent<Player>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(TagName.Player))
        {
            player = null;
        }
    }
}
