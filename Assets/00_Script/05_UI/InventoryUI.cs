using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryText;

    private void Start()
    {
        ChangeText(Player.Instance.Item);
    }

    private void OnEnable()
    {
        Player.Instance.OnItemChanged += ChangeText;
    }
    private void OnDisable()
    {
        //오류 로그 방지
        if (Player.Instance != null)
        {
            Player.Instance.OnItemChanged -= ChangeText;
        }
    }
    public void ChangeText(Item item)
    {
        inventoryText.text = item.ToString();
    }
}
