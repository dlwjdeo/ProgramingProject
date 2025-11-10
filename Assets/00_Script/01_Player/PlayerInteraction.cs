using System.Collections.Generic;
using UnityEngine;

//현재 플레이어와 상호작용 가능한 오브젝트의 Interaction 실행하는 class
public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private PlayerInputReader playerInputReader;

    private void Awake()
    {
        playerInputReader = GetComponentInParent<PlayerInputReader>();
    }

    //Inteactable 클래스를 가지고 있는 오브젝트를 식별
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

            if (obj.Priority > maxPriority) //만약 우선도가 같다면 먼저 들어온것부터
            {
                maxPriority = obj.Priority;
                highest = obj;
            }
        }

        return highest;
    }
}
