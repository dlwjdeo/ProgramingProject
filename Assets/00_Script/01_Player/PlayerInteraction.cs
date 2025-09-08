using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� Interaction �����ϴ� class
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable interactable;
    private PlayerInputReader playerInputReader;

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
    }

    //IInteactable �������̽��� ������ �ִ� ������Ʈ�� �ĺ�
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
