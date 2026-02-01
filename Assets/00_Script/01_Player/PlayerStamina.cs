using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float recoverRate = 10f;
    [SerializeField] private float decreaseRate = 15f;
    [SerializeField] private float minStaminaToRun = 0.1f;  // 달리기 가능한 최소 스테미나

    public float Current { get; private set; }
    public bool IsEmpty => Current <= 0f;
    public bool CanRun => Current >= minStaminaToRun;  // 달리기 가능한지 확인

    public event Action<float> OnStaminaChanged;

    private void Awake()
    {
        Current = maxStamina;
    }

    private void Start()
    {
        OnStaminaChanged?.Invoke(Current / maxStamina);
    }

    public void Decrease(float deltaTime)
    {
        setStamina(Current - decreaseRate * deltaTime);
    }

    public void Consume(float amount)
    {
        setStamina(Current - amount);
    }

    public void Recover(float deltaTime)
    {
        setStamina(Current + recoverRate * deltaTime);
    }

    private void setStamina(float value)
    {
        float prev = Current;
        Current = Mathf.Clamp(value, 0, maxStamina);

        if (!Mathf.Approximately(prev, Current))
            OnStaminaChanged?.Invoke(Current / maxStamina);
    }
}