using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;
    public GameState m_lastState;
    public GameState m_gameState;
    public event Action<GameState> OnGameStateChanged;
    public PlayerController m_player;    
    public MenuManager m_menuManager;
    public AudioManager m_audioManager;
    public AIManager m_aiManager;
    
    public List<Interactable> m_cinematicTriggers;
    public List<Transform> m_cinematicPoints;

    [HideInInspector]
    public Transform m_respawnPoint;
    public float m_respawnHealth;
    public float m_respawnMana;

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
    }

    private void Start()
    {
        UpdateGameState(GameState.TITLE);
    }

    void GameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.TITLE:
                TitleScreen();
                break;            
            case GameState.GAME:
                if (m_lastState == GameState.MENU)
                {
                    StartGame();
                }
                else if (m_lastState == GameState.PAUSE)
                {
                    ResumeGame();
                }                
                break;
            case GameState.CINEMATIC:
                break;
            case GameState.PAUSE:               
                break;
            case GameState.DEATH:
                DeathMenuStart();
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
            case GameState.MENU:
                break;
            case GameState.GAME:
                break;
            case GameState.CINEMATIC:
                break;
            case GameState.PAUSE:
                break;
            case GameState.OPTIONS:
                break;
            case GameState.CREDITS:
                break;
            case GameState.DEATH:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
        OnGameStateChanged?.Invoke(m_gameState);
    }

    void TitleScreen()       
    {
        //AudioManager.Instance.PlayMenuMusic(true);
        Time.timeScale = 0;        
    }

    void StartGame()
    {        
        Time.timeScale = 1;
    }

    void ResumeGame()
    {     
        Time.timeScale = 1;
    }

    public void SetRespawn(Transform transform)
    {
        m_respawnPoint = transform;
    }

    public void SetCheckPoint(Transform transform)
    {
        m_respawnPoint = transform;
    }   

    public async void CinematicTrigger(Interactable interactable) 
    {
        int index = m_cinematicTriggers.IndexOf(interactable);

        m_respawnPoint = m_cinematicPoints[index];
        m_player.transform.position = m_cinematicPoints[index].position;
        m_player.m_camera.SetRotation(m_cinematicPoints[index].rotation.eulerAngles);
        
        m_player.m_animator.SetTrigger("Cinematic");        
        m_audioManager.PlayVoiceSequence(interactable.m_audioReference);
        UpdateGameState(GameState.CINEMATIC);
    }    

    void DeathMenuStart()
    {
        m_player.m_animator.SetTrigger("Die");
        m_menuManager.FadeDeathScreen();
    }

    public void Respawn()
    {
        m_player.transform.position = m_respawnPoint.position;
        m_player.m_camera.SetRotation(m_respawnPoint.rotation.eulerAngles);
        m_player.m_currentHealth = 30;
        m_player.m_currentMana = 30;
        m_player.m_animator.SetTrigger("Alive");
        m_menuManager.UpdateHealth();
        m_menuManager.UpdateMana();
        m_aiManager.ResetEnemies();
        UpdateGameState(GameState.GAME);
    }
}

public enum GameState
{
    TITLE,
    MENU,
    GAME,
    CINEMATIC,
    PAUSE,
    OPTIONS,
    CREDITS,
    DEATH,
}