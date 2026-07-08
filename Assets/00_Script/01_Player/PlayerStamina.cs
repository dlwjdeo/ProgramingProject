using UnityEngine;
using System;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float recoverRate = 10f;
    [SerializeField] private float decreaseRate = 15f;
    [SerializeField] private float minStaminaToRun = 5f;  // 달리기 가능한 최소 스테미나

    public float Current { get; private set; }
    public bool IsEmpty => Current <= 1f;
    public bool CanRun => canRun;

    public event Action<float> OnStaminaChanged;

    private bool exhaustedSoundPlayed = false;
    private bool canRun = true;

    private void Awake()
    {
        Current = maxStamina;
        exhaustedSoundPlayed = false;
        canRun = true;
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
        
        // 회복시작하면 exhausted 플래그 리셋
        if (Current > 0f)
            exhaustedSoundPlayed = false;
    }

    private void setStamina(float value)
    {
        float prev = Current;
        Current = Mathf.Clamp(value, 0, maxStamina);
        
        if (IsEmpty)
            canRun = false;
        else if (!canRun && Current >= minStaminaToRun)
            canRun = true;

        if (!Mathf.Approximately(prev, Current))
        {
            OnStaminaChanged?.Invoke(Current / maxStamina);

            // 스테미나 0되었을 때 한 번만 exhausted 소리
            if (IsEmpty && !exhaustedSoundPlayed)
            {
                if (SoundManager.Instance != null)
                    SoundManager.Instance.PlayPlayerExhausted();
                exhaustedSoundPlayed = true;
            }
        }
    }
}