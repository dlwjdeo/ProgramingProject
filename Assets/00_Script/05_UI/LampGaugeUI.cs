using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LampGaugeUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private LampController lampController;

    private void Awake()
    {
        slider.maxValue = lampController.MaxLamp;
        slider.value = lampController.CurrentLamp;
    }

    private void Update()
    {
        slider.value = lampController.CurrentLamp;
    }

}
