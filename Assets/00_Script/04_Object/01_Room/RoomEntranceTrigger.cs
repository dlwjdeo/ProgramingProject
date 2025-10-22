using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEntranceTrigger : MonoBehaviour
{
    [SerializeField] private RoomController ownerRoom;

    private void Awake()
    {
        ownerRoom = GetComponentInParent<RoomController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.SetPlayerRoom(ownerRoom);
        }
        else if (other.CompareTag("Enemy"))
        {
            RoomManager.Instance.SetEnemyRoom(ownerRoom);
        }
    }
}
