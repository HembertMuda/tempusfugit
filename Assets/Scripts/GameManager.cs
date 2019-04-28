using System;
using UnityEngine;

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

    private void Start()
    {
        Cursor.SetCursor(Resources.Load("LD_Icons_Curseur_Small") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
