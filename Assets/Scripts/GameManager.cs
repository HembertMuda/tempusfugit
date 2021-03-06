﻿using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public enum GameState
    {
        Menu,
        Walking,
        Talking,
        SayingMemory
    }

    [SerializeField, BoxGroup("Memories")]
    public List<Memory> Memories = new List<Memory>();

    public Action<GameState> onGameStateChanged;

    public GameState CurrentGameState { get; set; } = GameState.Menu;

    private Texture2D cursor;

    public void ChangeState(GameState newGameState)
    {
        if (CurrentGameState != newGameState)
        {
            if (newGameState == GameState.Walking && CurrentGameState == GameState.Talking)
                StartCoroutine(WaitChageState(newGameState));
            else
            {
                CurrentGameState = newGameState;
                Cursor.lockState = CurrentGameState == GameState.Talking ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = CurrentGameState == GameState.Talking;
                Cursor.SetCursor(CurrentGameState == GameState.Talking ? cursor : null, Vector2.zero, CursorMode.ForceSoftware);

                if (onGameStateChanged != null)
                {
                    onGameStateChanged(newGameState);
                }
            }
        }
    }

    private IEnumerator WaitChageState(GameState newGameState)
    {
        yield return new WaitForSeconds(0.2f);

        CurrentGameState = newGameState;
        Cursor.lockState = CurrentGameState == GameState.Talking ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = CurrentGameState == GameState.Talking;
        Cursor.SetCursor(CurrentGameState == GameState.Talking ? cursor : null, Vector2.zero, CursorMode.ForceSoftware);

        if (onGameStateChanged != null)
        {
            onGameStateChanged(newGameState);
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    protected override void Awake()
    {
        DestroyOnLoad = true;
        base.Awake();
    }

    private void Start()
    {
        cursor = Resources.Load("LD_Icons_Curseur_Small") as Texture2D;
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.ForceSoftware);
        SoundManager.Instance.FadeMusic(false);
        //Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public Memory GetMemoryByName(string name)
    {
        for (int i = 0; i < Memories.Count; i++)
        {
            if (Memories[i].Name == name)
                return Memories[i];
        }

        Debug.LogError($"The memory {name} does not exist, check memories name on GameManager");

        return null;
    }
}
