using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState CurrentState {  get; private set; } = GameState.Playing;
    public Player Player => Player.Instance;

    public void SetGameState(GameState state) => CurrentState = state;

    public void GameOver()
    {
        UIManager.Instance.ShowGameOver();
    }
}
