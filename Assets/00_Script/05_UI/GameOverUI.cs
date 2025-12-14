using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private bool isShown;

    private void Awake()
    {
        panel.SetActive(false);
        isShown = false;
    }
    public void Show()
    {
        if (isShown) return;

        isShown = true;
        panel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void Hide()
    {
        isShown = false;
        panel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void OnClickRetry()
    {
        if (!isShown) return;
        
        Time.timeScale = 1f;
        Hide();
        SceneLoader.Instance.LoadScene("InGame");
    }

    public void OnClickGoTitle()
    {
        if (!isShown) return;

        Time.timeScale = 1f;
        Hide();
        SceneLoader.Instance.LoadScene("Title");
    }
}
