using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampController : MonoBehaviour
{
    private PlayerInputReader playerInputReader;

    [Header("램프 설정")]
    [SerializeField] private Light2D lamp;         
    [SerializeField] private float maxLamp = 30f;  
    [SerializeField]private float currentLamp = 30f;
    [SerializeField] private float decayRate = 1f;

    private void Awake()
    {
        playerInputReader = GetComponent<PlayerInputReader>();

        currentLamp = maxLamp;
        turnOff();
    }

    private void OnEnable()
    {
        playerInputReader.Lamp += lampToggle;
    }

    private void OnDisable()
    {
        playerInputReader.Lamp -= lampToggle;
    }

    private void Update()
    {
        if (lamp.enabled && currentLamp > 0)
        {
            useLamp();
        }
    }

    private void turnOn()
    {
        if (currentLamp <= 0) return;
        lamp.enabled = true;
    }

    private void turnOff()
    {
        lamp.enabled = false;
    }

    private void lampToggle()
    {
        if (lamp.enabled)
            turnOff();
        else
            turnOn();
    }

    private void useLamp()
    {
        currentLamp -= decayRate * Time.deltaTime;
        currentLamp = Mathf.Clamp(currentLamp, 0, maxLamp);

        if (currentLamp <= 0 && lamp.enabled)
        {
            turnOff();
        }
    }

    // 외부에서 충전 API
    public void ChargeLamp(float amount)
    {
        currentLamp += amount;
        currentLamp = Mathf.Min(currentLamp, maxLamp);
    }
}
