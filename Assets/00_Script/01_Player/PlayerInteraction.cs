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
        playerInputReader.Interaction += interact;
    }

    private void OnDisable()
    {
        playerInputReader.Interaction -= interact;
    }

    private void interact()
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

            if (obj == interactables[0]) // 가장 높은 Priority를 가진 Interactable에 OutlineMaterial 적용
            {
                obj.SetInteractable();
            }
            else // 나머지 Interactable은 OutlineMaterial 제거
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
