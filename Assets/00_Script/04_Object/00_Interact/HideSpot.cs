using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSpot : Interactable
{

    public override void Interact()
    {
        if(player != null)
        {
            player.PlayerStateMachine.ChangeState(player.PlayerStateMachine.Hide);
        }
    }
}
