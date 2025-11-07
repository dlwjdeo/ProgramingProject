using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float recoverRate = 10f;
    [SerializeField] private float decreaseRate = 15f;

    public float Current;// { get; private set; }
    public bool IsEmpty => Current <= 0f;

    public event Action<float> OnStaminaChanged;

    private void Awake()
    {
        Current = maxStamina;
        NotifyChange();
    }

    public void Decrease(float deltaTime)
    {
        SetStamina(Current - decreaseRate * deltaTime);
    }

    public void Consume(float amount)
    {
        SetStamina(Current - amount);
    }

    public void Recover(float deltaTime)
    {
        SetStamina(Current + recoverRate * deltaTime);
    }

    private void SetStamina(float value)
    {
        float prev = Current;
        Current = Mathf.Clamp(value, 0, maxStamina);

        if (!Mathf.Approximately(prev, Current))
            NotifyChange();
    }

    private void NotifyChange()
    {
        OnStaminaChanged?.Invoke(Current / maxStamina);
    }
}