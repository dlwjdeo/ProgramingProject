using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Video;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState {  get; private set; } = GameState.Default;
    public Player Player => Player.Instance;

    [FormerlySerializedAs("videoPlayer")]
    [SerializeField] private VideoPlayer PlayerDie;
    [SerializeField] private VideoPlayer EndingVideo;
    public UnityEvent OnVideoFinished;

    private void OnEnable()
    {
        if (PlayerDie != null)
            PlayerDie.loopPointReached += HandleVideoFinished;
        if (EndingVideo != null)
            EndingVideo.loopPointReached += HandleVideoFinished;
        OnVideoFinished.AddListener(FadeoutAndRestart);
    }

    private void OnDisable()
    {
        if (PlayerDie != null)
            PlayerDie.loopPointReached -= HandleVideoFinished;
        if (EndingVideo != null)
            EndingVideo.loopPointReached -= HandleVideoFinished;
        OnVideoFinished.RemoveListener(FadeoutAndRestart);
    }

    private void HandleVideoFinished(VideoPlayer source)
    {
        OnVideoFinished?.Invoke();
    }

    public void SetGameState(GameState state) => CurrentState = state;

    public void GameOver()
    {
        //UIManager.Instance.ShowGameOver();
        UIManager.Instance.ShowDeadPanel();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(0f); // BGM 볼륨을 0으로 설정하여 음악을 끕니다.
            SoundManager.Instance.SetSFXVolume(0f); // SFX 볼륨
        }

        if (PlayerDie != null)
            PlayerDie.Play();
    }

    public void PlayEndingVideo()
    {
        UIManager.Instance.ShowEndingPanel();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetBGMVolume(0f);
            SoundManager.Instance.SetSFXVolume(0f);
        }

        if (EndingVideo != null)
            EndingVideo.Play();
        else
            FadeoutAndRestart();
    }

    private void FadeoutAndRestart()
    {
        StartCoroutine(FadeOutAndRestartCoroutine());
    }
    private IEnumerator FadeOutAndRestartCoroutine()
    {
        yield return UIManager.Instance.FadeOut();
        SceneLoader.Instance.LoadScene("Title");
    }
}
