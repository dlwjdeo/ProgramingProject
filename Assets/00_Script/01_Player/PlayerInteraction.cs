using System.Collections.Generic;
using UnityEngine;

//���� �÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� Interaction �����ϴ� class
public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private Interactable interactable;
    private PlayerInputReader playerInputReader;

    private void Awake()
    {
        playerInputReader = GetComponentInParent<PlayerInputReader>();
    }

    //Inteactable Ŭ������ ������ �ִ� ������Ʈ�� �ĺ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            if (!interactables.Contains(interactable))
            { 
                interactables.Add(interactable); 
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            interactables.Remove(interactable);
        }
    }

    private void OnEnable()
    {
        playerInputReader.Interaction += interact;
    }

    private void OnDisable()
    {
        playerInputReader.Interaction -= interact;
    }

    private void interact()
    {
        if (interactables.Count == 0) return;

        var target = getPriorityInteractable(interactables);
        if(target != null)
        {
            target.Interact();
        }
    }

    private Interactable getPriorityInteractable(List<Interactable> interactables)
    {
        Interactable highest = null;
        int maxPriority = int.MinValue;

        foreach (var obj in interactables)
        {
            if (obj == null) continue;

            if (obj.Priority > maxPriority) //���� �켱���� ���ٸ� ���� ���°ͺ���
            {
                maxPriority = obj.Priority;
                highest = obj;
            }
        }

        return highest;
    }
}
