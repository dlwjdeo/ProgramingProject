using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("��Ż �̵� ��ǥ����")]
    [SerializeField] private Transform targetPoint;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // �÷��̾� ��ġ �̵�
            player.transform.position = targetPoint.position;
        }
    }
}
