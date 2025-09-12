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
        Player.Instance.OnItemChanged -= ChangeText;
    }
    public void ChangeText(Item item)
    {
        inventoryText.text = item.ToString();
    }
}
