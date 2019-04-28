using System;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public enum GameState
    {
        Walking,
        Talking,
        SayingMemory
    }

    public Action<GameState> onGameStateChanged;

    public GameState CurrentGameState { get; set; } = GameState.Walking;

    private Texture2D cursor;

    public void ChangeState(GameState newGameState)
    {
        if (CurrentGameState != newGameState)
        {
            CurrentGameState = newGameState;
            Cursor.lockState = CurrentGameState == GameState.Talking ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.SetCursor(CurrentGameState == GameState.Talking ? cursor : null, Vector2.zero, CursorMode.ForceSoftware);

            if (onGameStateChanged != null)
            {
                onGameStateChanged(newGameState);
            }

        }
    }

    private void Start()
    {
        cursor = Resources.Load("LD_Icons_Curseur_Small") as Texture2D;
        //Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }
}
