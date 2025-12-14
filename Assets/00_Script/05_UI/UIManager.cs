using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> 
{
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InterationMessageUI interationMessageUI;
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private GameOverUI gameOverUI;
    public IEnumerator FadeIn()
    {
        yield return screenFader.FadeIn();
    }

    public IEnumerator FadeOut()
    {
        yield return screenFader.FadeOut();
    }
    public void ShowMessage(string message) => interationMessageUI.ShowMessage(message);

    public void StartDialog(DialogSequence sequence) => dialogManager.StartDialog(sequence);

    public void ShowGameOver() => gameOverUI.Show();
}
