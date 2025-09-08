using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//현재 플레이어와 상호작용 가능한 오브젝트의 Interaction 실행하는 class
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable interactable;
    private PlayerInputReader playerInputReader;

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
    }

    //IInteactable 인터페이스를 가지고 있는 오브젝트를 식별
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IInteractable>(out var interactableObject))
        {
            interactable = interactableObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IInteractable>(out var interactableObject) && interactable != null)
        {
            interactable = null;
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
        interactable.Interact();
    }
}
