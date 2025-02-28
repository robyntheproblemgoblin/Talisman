using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Systems;
using UnityEngine.UIElements;

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
    public float m_talismanFadeWhite = 1f;
    public float m_talismanFadeBlack = 1f;
    public float m_talismanFadeClear = 1f;
    public float m_moveToCinematicSpeed = 1f;

    [HideInInspector]
    public Transform m_respawnPoint;
    [HideInInspector]
    public Transform m_initialSpawn;
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
        m_aiManager = new AIManager();
        OnGameStateChanged += GameStateChanged;
    }

    private void Start()
    {
        UpdateGameState(GameState.MENU);        
    }

    void GameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.MENU:
                MainMenu();
                break;
            case GameState.GAME:

                ResumeGame();

                break;
            case GameState.CINEMATIC:
                break;
            case GameState.PAUSE:
                PauseGame();
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
            case GameState.CONTROLS:
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

    void MainMenu()
    {
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

    void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void SetCheckPoint(Transform transform)
    {
        m_respawnPoint = transform;
    }

    public async UniTask FirstCinematic()
    {
        UpdateGameState(GameState.CINEMATIC);
        m_player.m_skinnedMeshRenderer.enabled = true;
        m_player.m_healParticles.gameObject.SetActive(true);
        Vector3 rotation = new Vector3(0, m_cinematicPoints[0].rotation.eulerAngles.y, 0);
       while (m_player.gameObject.transform.position != m_cinematicPoints[0].position &&
           m_player.gameObject.transform.rotation.eulerAngles != rotation)
       {
           float step = m_moveToCinematicSpeed * Time.deltaTime;
           Vector3 nextPos = Vector3.MoveTowards(m_player.gameObject.transform.position, m_cinematicPoints[0].position, step);
           Vector3 nextRot = Vector3.RotateTowards(m_player.gameObject.transform.forward, m_cinematicPoints[0].rotation.eulerAngles, step, step);
           m_player.gameObject.transform.position = nextPos;
            m_player.m_camera.MoveCamera(nextRot, m_player.m_cameraSensitivity);
            await UniTask.Yield();
        }    
    m_player.m_animator.SetTrigger("TalismanCinematic");
        m_audioManager.PlayCinematic().Forget();
    }

    public async UniTask SecondCinematic()
    {
        m_menuManager.m_fadeWhite.gameObject.SetActive(true);
        Color w;
        Color f;
        Color b = new Color(0, 0, 0, 1);
        while (m_menuManager.m_fadeWhite.color.a <= 1)
        {
            w = new Color(1, 1, 1, m_menuManager.m_fadeWhite.color.a + Time.deltaTime * m_talismanFadeWhite);
            m_menuManager.m_fadeWhite.color = w;
            await UniTask.Yield();
        }
        w = new Color(1, 1, 1, 1);
        m_menuManager.m_fadeWhite.color = w;
        m_menuManager.m_fadeBlack.gameObject.SetActive(true);
        m_menuManager.m_fadeBlack.color = b;

        m_respawnPoint = m_cinematicPoints[1];
        m_player.transform.position = m_cinematicPoints[1].position;
        m_player.m_camera.SetRotation(m_cinematicPoints[1].rotation.eulerAngles);

        while (m_menuManager.m_fadeWhite.color.a >= 0)
        {
            f = new Color(1, 1, 1, m_menuManager.m_fadeWhite.color.a - Time.deltaTime * m_talismanFadeBlack);
            m_menuManager.m_fadeWhite.color = f;
            await UniTask.Yield();
        }
        f = new Color(1, 1, 1, 0);
        m_menuManager.m_fadeWhite.color = f;

        m_player.m_animator.SetTrigger("SwordCinematic");
        m_audioManager.PlayCinematic().Forget();


        while (m_menuManager.m_fadeBlack.color.a >= 0)
        {
            b = new Color(1, 1, 1, m_menuManager.m_fadeBlack.color.a - Time.deltaTime * m_talismanFadeClear);
            m_menuManager.m_fadeBlack.color = b;
            await UniTask.Yield();
        }
        b = new Color();
        m_menuManager.m_fadeWhite.color = b;
        m_menuManager.m_fadeWhite.gameObject.SetActive(false);
        m_menuManager.m_fadeBlack.gameObject.SetActive(false);
    }

    public void LastCinematic()
    {
        UpdateGameState(GameState.CINEMATIC);
        m_player.m_animator.SetTrigger("AltarCinematic");
        m_audioManager.PlayCinematic().Forget();
    }

    public async UniTask EndGame()
    {
        UpdateGameState(GameState.DEATH);
        m_menuManager.FadeDeathScreen().Forget();
    }

    void DeathMenuStart()
    {
        m_player.m_animator.SetTrigger("Die");
        m_audioManager.PlayDeathDialogue();
        m_menuManager.FadeDeathScreen().Forget();
    }

    public void Respawn()
    {
        UpdateGameState(GameState.GAME);
        m_player.m_animator.SetTrigger("Alive");
        m_player.transform.position = m_respawnPoint.position;
        m_player.m_camera.SetRotation(m_respawnPoint.rotation.eulerAngles);
        m_player.m_currentHealth = m_player.m_health;
        m_player.m_currentMana = m_player.m_startMana;
        m_menuManager.UpdateHealth();
        m_menuManager.UpdateMana();
        m_aiManager.ResetEnemies();
    }
}

public enum GameState
{
    CONTROLS,
    MENU,
    GAME,
    CINEMATIC,
    PAUSE,
    OPTIONS,
    CREDITS,
    DEATH,
}