using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InterationMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float displayDuration = 2f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        messageText.text = "";
        messageText.gameObject.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        messageText.gameObject.SetActive(false);
        messageText.text = "";
        currentCoroutine = null;
    }
}
