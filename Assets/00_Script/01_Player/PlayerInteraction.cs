using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���� �÷��̾�� ��ȣ�ۿ� ������ ������Ʈ�� Interaction �����ϴ� class
public class PlayerInteraction : MonoBehaviour
{
    private IInteractable _interactable;
    private PlayerInputReader _playerInputReader;

    private void Awake()
    {
        _playerInputReader = GetComponent<PlayerInputReader>();
    }

    //IInteactable �������̽��� ������ �ִ� ������Ʈ�� �ĺ�
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IInteractable>(out var interactable))
        {
            _interactable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IInteractable>(out var interactable) && _interactable != null)
        {
            _interactable = null;
        }
    }

    private void OnEnable()
    {
        _playerInputReader.Interaction += Interact;
    }

    private void OnDisable()
    {
        _playerInputReader.Interaction -= Interact;
    }

    private void Interact()
    {
        _interactable.Interact();
    }
}
