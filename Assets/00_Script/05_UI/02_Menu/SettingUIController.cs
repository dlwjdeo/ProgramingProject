using UnityEngine;

public class SettingsUIController : MonoBehaviour
{
    public static SettingsUIController Instance { get; private set; }

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private bool pauseGame = true;

    private bool isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;
        settingsPanel.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f;
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        settingsPanel.SetActive(false);

        if (pauseGame)
            Time.timeScale = 1f;
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
}

