using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    //TODO: Strategy 패턴 구현 점검

    [SerializeField] private Item KeyItem;
    private Player player; //플레이어가 현재 가지고 있는 아이템 확인용
    public void Interact()
    {
        //TODO: 실질적인 작동 내용 작성
        Debug.Log("상자 작동");
        if(player != null && player.item == KeyItem)
        {
            Debug.Log("열렸습니다!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagName.Player))
        {
            player = collision.GetComponent<Player>();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(TagName.Player))
        {
            player = null;
        }
    }
}
