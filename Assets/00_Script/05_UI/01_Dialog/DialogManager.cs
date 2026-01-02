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

    [Header("Typing")]
    [SerializeField] private float typingSpeed = 0.03f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 3f;

    private List<DialogLine> lines;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;
    private Coroutine fadeCoroutine;

    private bool isRunning = false;

    private void Awake()
    {
        // ÃÊ±â ¼û±è
        SetVisible(false, immediate: true);
    }

    private void OnEnable()
    {
        if (Player.Instance != null && Player.Instance.PlayerInputReader != null)
            Player.Instance.PlayerInputReader.Dialog += NextLine;
    }

    private void OnDisable()
    {
        if (Player.Instance != null && Player.Instance.PlayerInputReader != null)
            Player.Instance.PlayerInputReader.Dialog -= NextLine;
    }

    public void StartDialog(DialogSequence sequence)
    {
        if (sequence == null || sequence.Lines == null || sequence.Lines.Count == 0) return;

        if (isRunning) return;

        isRunning = true;

        GameManager.Instance.SetGameState(GameState.Dialog);

        lines = sequence.Lines;
        currentIndex = 0;

        if (Player.Instance != null && Player.Instance.PlayerMover != null)
        {
            Player.Instance.PlayerMover.SetMoveEnabled(false);
            Player.Instance.PlayerMover.SetMoveInput(Vector2.zero);
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInThenShowFirst());
    }

    private IEnumerator FadeInThenShowFirst()
    {
        SetVisible(true, immediate: false);
        yield return Fade(0f, 1f);

        ShowCurrentLine();
    }

    private void NextLine()
    {
        if (!isRunning || lines == null) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            dialogText.text = lines[currentIndex].Text;
            return;
        }

        currentIndex++;

        if (currentIndex < lines.Count)
        {
            ShowCurrentLine();
        }
        else
        {
            EndDialog();
        }
    }

    private void ShowCurrentLine()
    {
        var line = lines[currentIndex];
        var speaker = line.Speaker;

        if (speaker != null)
        {
            nameText.text = speaker.SpeakerName;

            if (speaker.Portrait != null)
            {
                portraitImage.enabled = true;
                portraitImage.sprite = speaker.Portrait;
            }
            else
            {
                portraitImage.enabled = false;
            }
        }
        else
        {
            nameText.text = "";
            portraitImage.enabled = false;
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(line.Text));
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

    private void EndDialog()
    {
        isRunning = false;

        // ÀÔ·Â/»óÅÂ º¹±¸
        GameManager.Instance.SetGameState(GameState.Playing);

        if (Player.Instance != null && Player.Instance.PlayerMover != null)
            Player.Instance.PlayerMover.SetMoveEnabled(true);

        // UI ÆäÀÌµå¾Æ¿ô
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutAndHide());
    }

    private IEnumerator FadeOutAndHide()
    {
        yield return Fade(1f, 0f);
        SetVisible(false, immediate: true);
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private void SetVisible(bool visible, bool immediate)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(visible);

        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;

        if (immediate)
            canvasGroup.alpha = visible ? 1f : 0f;
    }
}
