using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Item dropItem;
    public override void Interact()
    {
        if(player != null && player.Item == keyItem)
        {
            ShowSuccess();
            //TODO:���� ������ ��� ����
        }
        else
        {
            ShowFail();
        }
    }
}
