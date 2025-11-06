using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : Interactable
{
    [SerializeField] private DialogSequence dialogSequence;
    public override void Interact()
    {
        UIManager.Instance.StartDialog(dialogSequence);
    }
}
