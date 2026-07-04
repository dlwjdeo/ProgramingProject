using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private PlayerInputReader playerInputReader;

    [SerializeField] private Material OutlineMaterial;
    [SerializeField] private Material DefaultMaterial;

    private void Awake()
    {
        playerInputReader = GetComponentInParent<PlayerInputReader>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            if (!interactables.Contains(interactable))
            { 
                interactables.Add(interactable);
                solting();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable))
        {
            interactables.Remove(interactable);
            solting();
            interactable.SetDefault();
        }
    }

    private void OnEnable()
    {
        playerInputReader.Interaction += Interact;
    }

    private void OnDisable()
    {
        playerInputReader.Interaction -= Interact;
    }

    public void Interact()
    {
        if (interactables.Count == 0) return;

        var target = interactables[0];
        if(target != null)
        {
            target.Interact();
        }
    }
    private void SetInteractables()
    {
        foreach (var obj in interactables)
        {
            if (obj == null) continue;

            if (obj == interactables[0]) // ���� ���� Priority�� ���� Interactable�� OutlineMaterial ����
            {
                obj.SetInteractable();
            }
            else // ������ Interactable�� OutlineMaterial ����
            {
                obj.SetDefault();
            }
        }
    }

    private void solting()
    {
        interactables.Sort((a, b) => b.Priority.CompareTo(a.Priority));
        SetInteractables();
    }

}
