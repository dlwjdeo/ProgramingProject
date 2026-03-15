using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inventoryText;
    private PlayerInventory playerInventory;

    private void Awake()
    {
        playerInventory = Player.Instance.PlayerInventory;
    }
    private void Start()
    {
        ChangeText(playerInventory.CurrentItem);
    }

    private void OnEnable()
    {
        playerInventory.OnItemChanged += ChangeText;
    }
    private void OnDisable()
    {
        //���� �α� ����
        if (playerInventory != null)
        {
            playerInventory.OnItemChanged -= ChangeText;
        }
    }
    public void ChangeText(Item item)
    {
        if (item == Item.Null)
        {
            inventoryText.text = "No Item";
            return;
        }
        inventoryText.text = item.ToString();
    }
}
