using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image portraitImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private float typingSpeed = 0.03f;

    private List<DialogLine> lines;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        if(Player.Instance != null) Player.Instance.PlayerInputReader.Dialog += NextLine;
    }

    private void OnDisable()
    {
        if(Player.Instance != null) Player.Instance.PlayerInputReader.Dialog -= NextLine;
    }

    // 대화 시작
    public void StartDialog(DialogSequence sequence)
    {
        if (sequence == null || sequence.Lines.Count == 0) return;

        GameManager.Instance.SetGameState(GameState.Dialog);
        lines = sequence.Lines;
        currentIndex = 0;
        ShowCurrentLine();
        StartCoroutine(FadeIn());
        Player.Instance.PlayerMover.SetMove(false);
    }

    // 다음 대사로 진행
    public void NextLine()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = lines[currentIndex].text;
            typingCoroutine = null;
            return;
        }

        currentIndex++;
        if (currentIndex < lines.Count - 1)
        {
            ShowCurrentLine();
        }
        else
        {
            GameManager.Instance.SetGameState(GameState.Playing);
            Player.Instance.PlayerMover.SetMove(true);
            StartCoroutine(FadeOut());
        }
    }

    private void ShowCurrentLine()
    {
        var line = lines[currentIndex];
        var speaker = line.speaker;

        if (speaker != null)
        {
            nameText.text = speaker.speakerName;

            if (speaker.portrait != null)
            {
                portraitImage.enabled = true;
                portraitImage.sprite = speaker.portrait;
            }
            else portraitImage.enabled = false;
        }
        else
        {
            nameText.text = "";
            portraitImage.enabled = false;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.text));
    }

    private IEnumerator TypeText(string text)
    {
        dialogText.text = "";
        foreach (char c in text)
        {
            dialogText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        typingCoroutine = null;
    }

    private IEnumerator FadeIn()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        canvasGroup.interactable = true;
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
    }
}
