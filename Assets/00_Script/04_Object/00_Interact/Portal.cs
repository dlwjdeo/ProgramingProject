using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("포탈 이동 목표지점")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private CameraArea targetArea;

    public override void Interact()
    {
        if (player != null && targetPoint != null)
        {
            // 플레이어 위치 이동
            player.transform.position = targetPoint.position;
            CameraManager.Instance.SwitchCamera(targetArea);
        }
    }
}
