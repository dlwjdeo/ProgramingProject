using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

//���� �÷��̾ �ִ� ���� ��ġ�� �������ִ� Manager
public class RoomManager : Singleton<RoomManager>
{

    [Header("�� ����")]
    [SerializeField] private List<RoomController> rooms = new List<RoomController>();
    public List<RoomController> Rooms => rooms;
    [SerializeField] private List<Portal> portals = new List<Portal>();
    
    public Portal FindClosestPortal(int fromFloor, int toFloor, Vector3 enemyPos)
    {
        // fromFloor�� toFloor�� ���̰� 2�̻��̸�, toFloor�� �� �� ���̷� ����
        if (fromFloor < toFloor)
        {
            toFloor = fromFloor + 1;
        }
        else if(fromFloor > toFloor)
        {
            toFloor = fromFloor - 1;
        }

        List<Portal> candidates = new List<Portal>();

        foreach (Portal portal in portals)
        {
            if (portal.FromFloor == fromFloor && portal.ToFloor == toFloor)
            {
                candidates.Add(portal);
            }
        }
        if (candidates.Count == 0) return null;

        float minDist = float.MaxValue;
        Portal closest = null;

        foreach (var portal in candidates)
        {
            float dist = Vector2.Distance(enemyPos, portal.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = portal;
            }
        }

        return closest;
    }
    public RoomController GetRandomRoom()
    {
        if (rooms == null) return null;
        int index = Random.Range(0, rooms.Count);
        return rooms[index];
    }
}