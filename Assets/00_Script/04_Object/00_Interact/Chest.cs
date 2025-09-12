using UnityEngine;

public class Chest : Interactable
{
    [SerializeField] private Item keyItem;
    [SerializeField] private Item dropItem;
    public override void Interact()
    {
        //TODO: 실질적인 작동 내용 작성
        Debug.Log("상자 작동");
        if(player != null && player.Item == keyItem)
        {
            Debug.Log("열렸습니다!");
        }
    }
}
