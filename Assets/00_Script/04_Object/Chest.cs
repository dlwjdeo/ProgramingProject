using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    //TODO: Strategy ���� ���� ����

    [SerializeField] private Item KeyItem;
    private Player player; //�÷��̾ ���� ������ �ִ� ������ Ȯ�ο�
    public void Interact()
    {
        //TODO: �������� �۵� ���� �ۼ�
        Debug.Log("���� �۵�");
        if(player != null && player.item == KeyItem)
        {
            Debug.Log("���Ƚ��ϴ�!");
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
