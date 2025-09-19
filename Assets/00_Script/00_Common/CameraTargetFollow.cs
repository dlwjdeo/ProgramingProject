using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float fixedY = 0f;

    void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, fixedY, player.position.z);
    }
}
