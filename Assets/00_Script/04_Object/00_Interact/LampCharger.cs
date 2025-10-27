using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampCharger : Interactable
{
    [Header("ÃæÀü·®")]
    [SerializeField] private float amount;
    public override void Interact()
    {
        if(player != null)
        {
            var lampController = player.GetComponent<LampController>();
            lampController.ChargeLamp(amount);
            ShowSuccess();
            Destroy(gameObject);
        }
    }
}
