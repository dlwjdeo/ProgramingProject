using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Item dropItem;
    public override void Interact()
    {
        //TODO: �������� �۵� ���� �ۼ�
        Debug.Log("���� �۵�");
        if(player != null && player.Item == keyItem)
        {
            Debug.Log("���Ƚ��ϴ�!");
        }
    }
}
