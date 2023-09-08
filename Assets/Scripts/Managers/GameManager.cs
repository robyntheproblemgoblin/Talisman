using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public GameState m_lastState;
    public GameState m_gameState;
    public event Action<GameState> OnGameStateChanged;
    public PlayerController m_player;
    public FPControls m_inputControl;

    Transform m_teleportPoint;

    public static GameManager Instance
    {
        get
        {
            m_instance = FindObjectOfType<GameManager>();
            if (m_instance == null)
            {
                Debug.LogError("Player Manager is Null!!");
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        OnGameStateChanged += GameStateChanged;
        m_player = FindObjectOfType<PlayerController>();
        m_inputControl = new FPControls();
        m_player.m_inputControl = m_inputControl;
        m_inputControl.Player_Map.Enable();
        
    }

    private void Start()
    {
        UpdateGameState(GameState.GAME);
        //Time.timeScale = 0;
    }

    void GameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.TITLE:
                //AudioManager.Instance.PlayMenuMusic(true);
                break;
            case GameState.GAME:
                Time.timeScale = 1;
                break;
            case GameState.CINEMATIC:
                break;
            case GameState.PAUSE:
                break;
            default:
                break;
        }
    }

    public void UpdateGameState(GameState newState)
    {
        if (newState == m_gameState)
            return;
        m_lastState = m_gameState;
        m_gameState = newState;
        switch (newState)
        {
            case GameState.TITLE:
                break;
            case GameState.GAME:
                break;
            case GameState.CINEMATIC:
                break;
            case GameState.PAUSE:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(m_gameState);
    }

    public void StartGame()
    {

    }

    public void GameOver()
    {
        
    }
}

public enum GameState
{
    GAME,
    TITLE,
    MENU,
    CINEMATIC,
    PAUSE,
    OPTIONS,
    CREDITS
}