using System;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public enum GameState
    {
        Walking,
        Talking
    }

    public Action<GameState> onGameStateChanged;

    public GameState CurrentGameState { get; set; } = GameState.Walking;

    public void ChangeState(GameState newGameState)
    {
        if (CurrentGameState != newGameState)
        {
            CurrentGameState = newGameState;
            if (onGameStateChanged != null)
            {
                onGameStateChanged(newGameState);
            }
        }
    }
}
