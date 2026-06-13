using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CinematicTimelineController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayableDirector playableDirector;

    [Header("Options")]
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool lockCameraManager = true;
    [SerializeField] private bool lockPlayerMovement = true;
    [SerializeField] private bool lockPlayerAnimator = true;

    private bool isCinematicPlaying;
    private bool hasPlayerBodyState;
    private RigidbodyType2D cachedBodyType;
    private Vector2 cachedVelocity;

    private void Awake()
    {
        if (playableDirector == null)
            playableDirector = GetComponent<PlayableDirector>();

        if (playableDirector != null)
        {
            playableDirector.played += OnDirectorPlayed;
            playableDirector.stopped += OnDirectorStopped;
            playableDirector.paused += OnDirectorPaused;
        }
    }

    private void Start()
    {
        if (playOnStart)
            PlayTimeline();
    }

    private void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.played -= OnDirectorPlayed;
            playableDirector.stopped -= OnDirectorStopped;
            playableDirector.paused -= OnDirectorPaused;
        }
    }

    [ContextMenu("Play Timeline")]
    public void PlayTimeline()
    {
        if (playableDirector == null)
            return;

        if (isCinematicPlaying)
            return;

        isCinematicPlaying = true;
        ApplyCinematicState(true);

        playableDirector.time = 0d;
        playableDirector.Play();
    }

    [ContextMenu("Stop Timeline")]
    public void StopTimeline()
    {
        if (playableDirector == null)
            return;

        playableDirector.Stop();
        EndCinematic();
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        if (director != playableDirector)
            return;

        EndCinematic();
    }

    private void OnDirectorPlayed(PlayableDirector director)
    {
        if (director != playableDirector)
            return;

        if (isCinematicPlaying)
            return;

        isCinematicPlaying = true;
        ApplyCinematicState(true);
    }

    private void OnDirectorPaused(PlayableDirector director)
    {
        if (director != playableDirector)
            return;

        // Some timelines end in paused state (e.g. Hold) without invoking stopped.
        if (isCinematicPlaying && IsTimelineAtEnd())
            EndCinematic();
    }

    private bool IsTimelineAtEnd()
    {
        if (playableDirector == null)
            return false;

        double duration = playableDirector.duration;
        if (double.IsNaN(duration) || double.IsInfinity(duration) || duration <= 0d)
            return false;

        return playableDirector.time >= duration - 0.01d;
    }

    private void EndCinematic()
    {
        if (!isCinematicPlaying)
            return;

        isCinematicPlaying = false;
        ApplyCinematicState(false);
    }

    private void ApplyCinematicState(bool active)
    {
        if (lockCameraManager && CameraManager.Instance != null)
            CameraManager.Instance.SetTimelineCameraControl(active);

        if (Player.Instance == null)
            return;

        if (lockPlayerMovement && Player.Instance.PlayerMover != null)
        {
            Player.Instance.PlayerMover.SetMoveEnabled(!active);

            if (active)
                Player.Instance.PlayerMover.SetMoveInput(Vector2.zero);

            Rigidbody2D rb = Player.Instance.Rigidbody2D;
            if (rb != null)
            {
                if (active)
                {
                    cachedBodyType = rb.bodyType;
                    cachedVelocity = rb.velocity;
                    hasPlayerBodyState = true;

                    // Prevent physics from overriding timeline transform animation.
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
                else if (hasPlayerBodyState)
                {
                    rb.bodyType = cachedBodyType;
                    rb.velocity = cachedVelocity;
                    hasPlayerBodyState = false;
                }
            }
        }

        if (lockPlayerAnimator && Player.Instance.PlayerAnimator != null)
            Player.Instance.PlayerAnimator.IsCinematicMode = active;
    }
}
