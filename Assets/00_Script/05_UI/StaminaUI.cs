using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider;

    private void Start()
    {
         Player.Instance.PlayerStamina.OnStaminaChanged += UpdateUI;
         UpdateUI(Player.Instance.PlayerStamina.Current / 100f); //초기값 반영
    }

    private void OnDisable()
    {
        if (Player.Instance != null && Player.Instance.PlayerStamina != null)
        {
            Player.Instance.PlayerStamina.OnStaminaChanged -= UpdateUI;
        }
    }

    private void UpdateUI(float ratio)
    {
        staminaSlider.value = ratio;
    }
}
