using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    private const string LogTag = "[Tutorial]";

    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Sprite[] closeFrames = new Sprite[3];
    [SerializeField] private Sprite[] openFrames = new Sprite[3];
    [SerializeField] private float frameDuration = 0.1f;

    private Player cachedPlayer;
    private bool isSubscribed;
    private Coroutine animationCoroutine;
    private Coroutine subscribeCoroutine;

    private void Awake()
    {
        EnsureRenderer();
        Debug.Log($"{LogTag} Awake - renderer={(targetRenderer != null ? targetRenderer.name : "null")}", this);
    }

    private void OnEnable()
    {
        Debug.Log($"{LogTag} OnEnable", this);
        StartSubscribeRoutine();
    }

    private void OnDisable()
    {
        Debug.Log($"{LogTag} OnDisable", this);
        StopSubscribeRoutine();
        StopCurrentAnimation();
        UnsubscribePlayerEvents();
    }

    private void Start()
    {
        EnsureRenderer();

        if (closeFrames != null && closeFrames.Length > 0)
        {
            targetRenderer.sprite = closeFrames[0];
        }
        else if (openFrames != null && openFrames.Length > 0)
        {
            targetRenderer.sprite = openFrames[0];
        }
    }

    public void PlayAnimation()
    {
        CloseAnimation();
    }

    public void OpenAnimation()
    {
        Debug.Log($"{LogTag} OpenAnimation called", this);
        StartAnimation(openFrames);
    }

    public void CloseAnimation()
    {
        Debug.Log($"{LogTag} CloseAnimation called", this);
        StartAnimation(closeFrames);
    }

    private void StartAnimation(Sprite[] frames)
    {
        EnsureRenderer();

        Debug.Log($"{LogTag} StartAnimation - renderer={(targetRenderer != null)}, framesLength={(frames == null ? -1 : frames.Length)}, frameDuration={frameDuration}", this);

        if (targetRenderer == null || frames == null || frames.Length == 0)
        {
            Debug.LogWarning($"{LogTag} StartAnimation aborted - invalid renderer or frames", this);
            return;
        }

        StopCurrentAnimation();
        animationCoroutine = StartCoroutine(PlayFramesCoroutine(frames));
        Debug.Log($"{LogTag} Coroutine started", this);
    }

    private IEnumerator PlayFramesCoroutine(Sprite[] frames)
    {
        Debug.Log($"{LogTag} Coroutine begin - firstFrame={(frames[0] != null ? frames[0].name : "null")}", this);
        targetRenderer.sprite = frames[0];

        for (int i = 1; i < frames.Length; i++)
        {
            yield return new WaitForSeconds(frameDuration);
            targetRenderer.sprite = frames[i];
            Debug.Log($"{LogTag} Frame applied index={i}, sprite={(frames[i] != null ? frames[i].name : "null")}", this);
        }

        animationCoroutine = null;
        Debug.Log($"{LogTag} Coroutine finished", this);
    }

    private void StopCurrentAnimation()
    {
        if (animationCoroutine == null)
        {
            return;
        }

        StopCoroutine(animationCoroutine);
        animationCoroutine = null;
        Debug.Log($"{LogTag} Coroutine stopped", this);
    }

    private void EnsureRenderer()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
            Debug.Log($"{LogTag} EnsureRenderer - auto assigned renderer={(targetRenderer != null ? targetRenderer.name : "null")}", this);
        }
    }

    private void SubscribePlayerEvents()
    {
        if (isSubscribed)
        {
            Debug.Log($"{LogTag} Subscribe skipped - already subscribed", this);
            return;
        }

        if (cachedPlayer == null)
        {
            cachedPlayer = FindObjectOfType<Player>();
            Debug.Log($"{LogTag} FindObjectOfType<Player> result={(cachedPlayer != null ? cachedPlayer.name : "null")}", this);
        }

        if (cachedPlayer == null || cachedPlayer.PlayerStateMachine == null || cachedPlayer.PlayerStateMachine.Hide == null)
        {
            Debug.LogWarning($"{LogTag} Subscribe aborted - player/state/hide is null", this);
            return;
        }

        cachedPlayer.PlayerStateMachine.Hide.OnEnterHideState += CloseAnimation;
        cachedPlayer.PlayerStateMachine.Hide.OnExitHideState += OpenAnimation;
        isSubscribed = true;
        Debug.Log($"{LogTag} Subscribed to hide events", this);
    }

    private void StartSubscribeRoutine()
    {
        if (subscribeCoroutine != null)
        {
            return;
        }

        subscribeCoroutine = StartCoroutine(SubscribeWhenReadyCoroutine());
    }

    private void StopSubscribeRoutine()
    {
        if (subscribeCoroutine == null)
        {
            return;
        }

        StopCoroutine(subscribeCoroutine);
        subscribeCoroutine = null;
    }

    private IEnumerator SubscribeWhenReadyCoroutine()
    {
        while (!isSubscribed)
        {
            SubscribePlayerEvents();

            if (isSubscribed)
            {
                break;
            }

            yield return null;
        }

        subscribeCoroutine = null;
    }

    private void UnsubscribePlayerEvents()
    {
        if (!isSubscribed || cachedPlayer == null || cachedPlayer.PlayerStateMachine == null || cachedPlayer.PlayerStateMachine.Hide == null)
        {
            Debug.Log($"{LogTag} Unsubscribe skipped - invalid state", this);
            return;
        }

        cachedPlayer.PlayerStateMachine.Hide.OnEnterHideState -= CloseAnimation;
        cachedPlayer.PlayerStateMachine.Hide.OnExitHideState -= OpenAnimation;
        isSubscribed = false;
        Debug.Log($"{LogTag} Unsubscribed from hide events", this);
    }
}
