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
        //오류 로그 방지
        if (playerInventory != null)
        {
            playerInventory.OnItemChanged -= ChangeText;
        }
    }
    public void ChangeText(Item item)
    {
        inventoryText.text = item.ToString();
    }
}
