using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hokora : Interactable
{
    public override void Interact()
    {
        StartCoroutine(UIManager.Instance.FadeOut());
    }
}
