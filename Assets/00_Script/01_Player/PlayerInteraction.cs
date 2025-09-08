using UnityEngine;

//���� �÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� Interaction �����ϴ� class
public class PlayerInteraction : MonoBehaviour
{
    private Interactable interactable;
    private PlayerInputReader playerInputReader;

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
    }

    //Inteactable Ŭ������ ������ �ִ� ������Ʈ�� �ĺ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Interactable>(out var interactableObject))
        {
            interactable = interactableObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<Interactable>(out var interactableObject) && interactable != null)
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
