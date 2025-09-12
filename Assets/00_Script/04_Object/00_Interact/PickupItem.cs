using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PickupItem : Interactable
{
    [SerializeField] private Item item;
    public override void Interact()
    {
        if (player != null)
        {
            player.ChangeItem(item);

            Destroy(gameObject);
        }
    }
}
