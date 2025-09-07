using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Item item = Item.Null;

    public void ChangeItem(Item changeItem)
    {
        item = changeItem;
    }

}
