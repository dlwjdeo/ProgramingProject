using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState {  get; private set; } = GameState.Playing;
    public Player Player => Player.Instance;

    [SerializeField] private VideoPlayer videoPlayer;

    public void SetGameState(GameState state) => CurrentState = state;

    public void GameOver()
    {
        //UIManager.Instance.ShowGameOver();
        UIManager.Instance.ShowDeadPanel();
        SoundManager.Instance.bgmVolume = 0f; // BGM 볼륨을 0으로 설정하여 음악을 끕니다.
        SoundManager.Instance.sfxVolume = 0f; // SFX 볼륨
        videoPlayer.Play();
    }
}
