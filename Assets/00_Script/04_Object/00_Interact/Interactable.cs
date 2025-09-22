using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //TODO: 오브젝트의 position값이 강제로 이동되었을 경우 방어코드 필요
    protected Player player;
    protected bool inRange;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<Player>(out var triggerPlayer))
        {
            player = triggerPlayer;
            inRange = true;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.TryGetComponent<Player>(out var triggerPlayer) && player != null)
        {
            player = null;
            inRange = false;
        }
    }

    public abstract void Interact();
}
