using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); 
    }
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeRoutine(1f, 0f));
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeRoutine(0f, 1f));
    }

    private IEnumerator FadeRoutine(float from, float to)
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.SmoothStep(from, to, time / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = to;
        fadeImage.color = color;
    }
}
