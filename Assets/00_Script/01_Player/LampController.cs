using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampController : MonoBehaviour
{
    private PlayerInputReader playerInputReader;
    [Header("램프 설정")]
    [SerializeField] private Light2D lamp;
    [SerializeField] private float maxLamp;
    [SerializeField] private float currentLamp;
    [SerializeField] private float drainRate;
    [SerializeField] private bool toggle;
    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();
    }

    private void Update()
    {
        if(toggle && currentLamp > 0)
        {
            useLamp();
        }
    }

    private void OnEnable()
    {
        playerInputReader.Lamp += lampToggle;
    }

    private void OnDisable()
    {
        playerInputReader.Lamp -= lampToggle;
    }
    private void turnOn()
    {
        lamp.enabled = true;
        toggle = true;
    }

    private void turnOff()
    {
        lamp.enabled = false;
        toggle = false;
    }

    private void lampToggle()
    {
        if (currentLamp <= 0) return;
        if (toggle)
        {
            turnOff();
        }
        else
        {
            turnOn();
        }
    }

    private void useLamp()
    {
        currentLamp -= drainRate * Time.deltaTime;
        currentLamp = Mathf.Clamp(currentLamp, 0, maxLamp);

        if(currentLamp <= 0)
        {
            turnOff();
        }
    }

    //API
    public void ChargeLamp(float amount)
    {
        currentLamp += amount;
        currentLamp = Mathf.Min(currentLamp, maxLamp);
    }
}
