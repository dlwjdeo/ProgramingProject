using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item KeyItem;
    public override void Interact()
    {
        //TODO: �������� �۵� ���� �ۼ�
        Debug.Log("���� �۵�");
        if(player != null && player.Item == KeyItem)
        {
            Debug.Log("���Ƚ��ϴ�!");
        }
    }
}
