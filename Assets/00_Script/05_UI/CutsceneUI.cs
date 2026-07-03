using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneUI : MonoBehaviour
{
    [System.Serializable]
    public class FillClip
    {
        public Image image;
        public float duration = 1f;
        public float delay = 0f;
    }

    [System.Serializable]
    public class MoveClip
    {
        public RectTransform target;
        public bool useCurrentPositionAsStart = true;
        public Vector2 startPosition;
        public Vector2 endPosition = new Vector2(200f, 0f);
        public float duration = 1f;
        public float delay = 0f;
        public bool loop = false;
        public bool pingPong = true;
    }

    [System.Serializable]
    public class SceneClip
    {
        public string sceneName = "Scene";
        public List<FillClip> fills = new List<FillClip>();
        public List<MoveClip> moves = new List<MoveClip>();
    }

    [Header("Play")]
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private int startSceneIndex = 0;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Cutscene Scenes")]
    [SerializeField] private List<SceneClip> scenes = new List<SceneClip>();

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayScene(startSceneIndex);
        }
    }

    private void OnDisable()
    {
        StopScene();
    }

    public void PlayScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= scenes.Count)
        {
            return;
        }

        StopAllCoroutines();
        StartCoroutine(CoPlayScene(scenes[sceneIndex]));
    }

    public void PlayStartScene()
    {
        PlayScene(startSceneIndex);
    }

    public void StopScene()
    {
        StopAllCoroutines();
    }

    private IEnumerator CoPlayScene(SceneClip scene)
    {
        for (int i = 0; i < scene.fills.Count; i++)
        {
            FillClip fill = scene.fills[i];
            if (fill == null || fill.image == null)
            {
                continue;
            }

            StartCoroutine(CoFillFromBottom(fill));
        }

        for (int i = 0; i < scene.moves.Count; i++)
        {
            MoveClip move = scene.moves[i];
            if (move == null || move.target == null)
            {
                continue;
            }

            StartCoroutine(CoMoveHorizontal(move));
        }

        yield return null;
    }

    private IEnumerator CoFillFromBottom(FillClip fill)
    {
        if (fill.delay > 0f)
        {
            yield return Wait(fill.delay);
        }

        fill.image.type = Image.Type.Filled;
        fill.image.fillMethod = Image.FillMethod.Vertical;
        fill.image.fillOrigin = (int)Image.OriginVertical.Bottom;
        fill.image.fillClockwise = true;
        fill.image.fillAmount = 0f;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, fill.duration);

        while (elapsed < duration)
        {
            elapsed += DeltaTime();
            fill.image.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        fill.image.fillAmount = 1f;
    }

    private IEnumerator CoMoveHorizontal(MoveClip move)
    {
        if (move.delay > 0f)
        {
            yield return Wait(move.delay);
        }

        Vector2 start = move.useCurrentPositionAsStart ? move.target.anchoredPosition : move.startPosition;
        Vector2 end = move.endPosition;
        float duration = Mathf.Max(0.01f, move.duration);

        do
        {
            yield return MoveBetween(move, start, end, duration);

            if (move.pingPong)
            {
                yield return MoveBetween(move, end, start, duration);
            }
        }
        while (move.loop);
    }

    private IEnumerator MoveBetween(MoveClip move, Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += DeltaTime();
            float t = Mathf.Clamp01(elapsed / duration);
            move.target.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
            yield return null;
        }

        move.target.anchoredPosition = to;
    }

    private float DeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }

    private object Wait(float seconds)
    {
        return useUnscaledTime ? new WaitForSecondsRealtime(seconds) : new WaitForSeconds(seconds);
    }
}
