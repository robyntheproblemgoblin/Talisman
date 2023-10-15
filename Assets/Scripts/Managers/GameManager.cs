using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Systems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.EventSystems;

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
        m_aiManager = new AIManager();                    
        SetCurrentDevice();        
        InputSystem.onDeviceChange += InputDeviceChanged;
        OnGameStateChanged += GameStateChanged;
    }

    private void Start()
    {
        UpdateGameState(GameState.TITLE);
    }

    void SetCurrentDevice()
    {       
        for (int i = 2; i < InputSystem.devices.Count-1; i++)
        {
            InputSystem.RemoveDevice(InputSystem.devices[i]);
        }

        if (InputSystem.devices.Count > 2)
        {
            var device = InputSystem.devices[2];

            if (device is XInputController)
            {
                Debug.Log("Xbox " + InputSystem.devices.Count);
            }
           /* else if (device is Keyboard || device is Mouse)
            {
                Debug.Log("Keyboard " + InputSystem.devices.Count);
            }*/
            else if (device is DualShockGamepad)
            {
                Debug.Log("PS " + InputSystem.devices.Count);
            }
            else if (device is SwitchProControllerHID)
            {
                Debug.Log("Switch " + InputSystem.devices.Count);
            }
            else
            {
                Debug.LogAssertion("Failure on InputSystem");
            }
        }
        /*if (InputSystem.devices[0].description.manufacturer == "Sony Interactive Entertainment")
        {
            //Sets UI scheme to PS
            Debug.Log("Playstation Controller Detected");
            m_menuManager.m_currentController = ControllerType.PS;
        }
        else
        {
            //Sets UI scheme to XB
            Debug.Log("Xbox Controller Detected");
            m_menuManager.m_currentController = ControllerType.XBOX;
        }*/
    }

    //Method called  when a device change event is fired
    public void InputDeviceChanged(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            //New device added
            case InputDeviceChange.Added:
                Debug.Log("New device added");
                SetCurrentDevice();
                /*   //Checks if is Playstation Controller
                   if (device.description.manufacturer == "Sony Interactive Entertainment")
                   {
                       //Sets UI scheme
                       Debug.Log("Playstation Controller Detected");
                       currentImageScheme.SetImagesToPlaystation();
                       controllerTypeChange.Invoke();
                   }
                   //Else, assumes Xbox controller
                   //device.description.manufacturer for Xbox returns empty string
                   else
                   {
                       Debug.Log("Xbox Controller Detected");
                       currentImageScheme.SetImagesToXbox();
                       controllerTypeChange.Invoke();
                   }*/
                break;

            //Device disconnected
            case InputDeviceChange.Disconnected:
                SetCurrentDevice();
                //controllerDisconnected.Invoke();
                Debug.Log("Device disconnected");
                break;

            //Familiar device connected
            case InputDeviceChange.Reconnected:
                //controllerReconnected.Invoke();
                SetCurrentDevice();
                Debug.Log("Device reconnected");

                //Checks if is Playstation Controller
                /*  if (device.description.manufacturer == "Sony Interactive Entertainment")
                  {
                      //Sets UI scheme
                      Debug.Log("Playstation Controller Detected");
                      currentImageScheme.SetImagesToPlaystation();
                      controllerTypeChange.Invoke();
                  }
                  //Else, assumes Xbox controller
                  //device.description.manufacturer for Xbox returns empty string
                  else
                  {
                      Debug.Log("Xbox Controller Detected");
                      currentImageScheme.SetImagesToXbox();
                      controllerTypeChange.Invoke();
                  }*/
                break;

            //Else
            default:
                break;
        }
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