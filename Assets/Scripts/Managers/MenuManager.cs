using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    #region UI Category Objects
    public GameObject m_title;
    public GameObject m_mainMenu;
    public GameObject m_hud;
    public GameObject m_cinematics;
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    #endregion

    #region Title Screen Fields
    [Space(5), Header("Title Screen"), Space(5)]
    public Button m_startButton;
    public Camera m_titleCamera;
    #endregion

    #region Main Menu Fields
    [Header("Main Menu"), Space(5)]
    public Button m_newGame;
    public Button m_menuOptions;
    public Button m_credits;
    public Button m_bonusArt;
    public Button m_quit;
    #endregion

    #region HUD Fields
    [Header("HUD"), Space(5)]
    public TextMeshProUGUI m_subtitles;
    public Slider m_health;
    public Slider m_mana;
    public TextMeshProUGUI m_tutorial;
    public Image m_interactImage;
    public TextMeshProUGUI m_interactText;
    #endregion

    #region Cinematics
    [Header("Cinematics"), Space(5)]
    public Image m_topBlackBar;
    public Image m_bottomBlackBar;
    public Image m_fadeBlack;
    public Image m_fadeWhite;
    #endregion

    #region Pause Menu Fields
    [Header("Pause Menu"), Space(5)]
    public Button m_resume;
    public Button m_pauseOptions;
    public Button m_revert;
    public Button m_quitMenu;
    #endregion

    #region Options 
    [Header("Options"), Space(5)]
    public Button m_camSensitivityButton;
    public Slider m_camSensitivitySlider;
    public Button m_subtitlesButton;
    public Slider m_subtitlesSlider;
    public Button m_vibrationButton;
    public Slider m_vibrationSlider;
    public Button m_masterVolumeButton;
    public Slider m_masterVolumeSlider;
    public Button m_musicVolumeButton;
    public Slider m_musicVolumeSlider;
    public Button m_sfxButton;
    public Slider m_sfxSlider;
    public Button m_dialogueButton;
    public Slider m_dialogueSlider;
    public Button m_defaults;
    public Button m_optionsBack;
    #endregion    

    GameManager m_game;
    PlayerController m_player;
    FPControls m_inputs;

    private void Awake()
    {
        m_game = GameManager.Instance;
        m_player = m_game.m_player;
        m_inputs = m_game.m_inputControl;

        m_health.maxValue = m_player.m_health;
        m_game.OnGameStateChanged += OnGameStateChanged;
        
    }

    private void Start()
    {
        UpdateState(GameState.TITLE);
    }

    void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.TITLE:
                UpdateState(state);
                break;
            case GameState.GAME:
                if (m_game.m_lastState == GameState.PAUSE)
                {
                    Resume();
                }
                else if (m_game.m_lastState == GameState.MENU)
                {
                    StartGame();
                }
                break;
            case GameState.PAUSE:
                Pause();
                break;

        }
    }

    void UpdateState(GameState state)
    {

    }

    void Resume()
    {

    }

    void StartGame()
    {

    }

    private void Pause()
    {
        
    }
}
