using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private Item KeyItem;
    private Player player;
    public void Interact()
    {
        if(player != null || player.item == KeyItem)
        {
            Debug.Log("¿­·È½À´Ï´Ù!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagName.Player))
        {
            player = GetComponent<Player>();
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
