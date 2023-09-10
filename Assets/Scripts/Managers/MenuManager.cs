using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    #region UI Category Objects
    public GameObject m_title;
    public GameObject m_mainMenu;
    public GameObject m_hud;
    public GameObject m_cinematics;
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    // TEMPORARY
    public GameObject m_falseEnd;
    public Button m_falseQuit;
    #endregion

    #region Title Screen Fields
    [Space(5), Header("Title Screen"), Space(5)]
    public Button m_startButton;
    //public Camera m_titleCamera;
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

    #region Cinematic Fields
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
    public Button m_revertCheckpoint;
    public Button m_quitMenu;
    #endregion

    #region Option Fields
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

    //Credits

    GameManager m_game;
    PlayerController m_player;    
    EventSystem m_eventSystem;

    private void Awake()
    {
        m_game = GameManager.Instance;
        m_player = m_game.m_player;        
        m_game.m_menuManager = this;
        m_eventSystem = FindObjectOfType<EventSystem>();

        m_game.OnGameStateChanged += OnGameStateChanged;
        m_falseQuit.onClick.AddListener(delegate () { QuitGame(); });

        //Title Screen Setup
        m_startButton.onClick.AddListener(delegate () { MainMenu(); });

        //Main Menu Setup
        m_newGame.onClick.AddListener(delegate () { StartGame(); });
        m_menuOptions.onClick.AddListener(delegate () { Options(); });
        m_credits.onClick.AddListener(delegate () { Credits(); });
        m_bonusArt.onClick.AddListener(delegate () { BonusArt(); });
        m_quit.onClick.AddListener(delegate () { QuitGame(); });

        //HUD Setup

        //Cinematic Setup

        //Pause Menu Setup
        m_resume.onClick.AddListener(delegate () { Resume(); });
        m_pauseOptions.onClick.AddListener(delegate () { Options(); });
        m_revertCheckpoint.onClick.AddListener(delegate () { RevertCheckpoint(); });
        m_quitMenu.onClick.AddListener(delegate () { MainMenu(); });

        //Options Setup
        m_camSensitivityButton.onClick.AddListener(delegate () { });
        m_camSensitivitySlider.SetValueWithoutNotify(m_player.m_cameraSensitivity);
        m_subtitlesButton.onClick.AddListener(delegate () { });
        m_subtitlesSlider.SetValueWithoutNotify(1);
        m_vibrationButton.onClick.AddListener(delegate () { });
        m_vibrationSlider.SetValueWithoutNotify(1);
        m_masterVolumeButton.onClick.AddListener(delegate () { });
        m_masterVolumeSlider.SetValueWithoutNotify(100);
        m_musicVolumeButton.onClick.AddListener(delegate () { });
        m_musicVolumeSlider.SetValueWithoutNotify(100);
        m_sfxButton.onClick.AddListener(delegate () { });
        m_sfxSlider.SetValueWithoutNotify(100);
        m_dialogueButton.onClick.AddListener(delegate () { });
        m_dialogueSlider.SetValueWithoutNotify(100);
        m_defaults.onClick.AddListener(delegate () { });
        m_optionsBack.onClick.AddListener(delegate () { });
    }
    private void Start()
    {        
    m_health.maxValue = m_player.m_health;        
    }

    void OnGameStateChanged(GameState state)
    {
        UpdateUI(state);
        switch (state)
        {
            case GameState.TITLE:
                TitleScreen();
                break;
            case GameState.MENU:
                MainMenu();
                break;
            case GameState.GAME:
                if (m_game.m_lastState == GameState.MENU)
                {
                    StartGame();
                }
                else if (m_game.m_lastState == GameState.PAUSE)
                {
                    Resume();
                }
                break;
            case GameState.PAUSE:
                Pause();
                break;
            case GameState.CINEMATIC:
                break;
            case GameState.CREDITS:
                break;
        }
    }

    void UpdateUI(GameState state)
    {
        m_title.SetActive(state == GameState.TITLE);
        m_mainMenu.SetActive(state == GameState.MENU);
        m_hud.SetActive(state == GameState.GAME);
        m_cinematics.SetActive(state == GameState.CINEMATIC);
        m_pauseMenu.SetActive(state == GameState.PAUSE);
        m_optionsMenu.SetActive(state == GameState.OPTIONS);
    }
    void TitleScreen()
    {
        m_eventSystem.SetSelectedGameObject(m_startButton.gameObject);
        Cursor.lockState = CursorLockMode.Confined;
    }
    void MainMenu()
    {
        m_game.UpdateGameState(GameState.MENU);
        m_eventSystem.SetSelectedGameObject(m_newGame.gameObject);
        Cursor.lockState = CursorLockMode.Confined;
    }
    void StartGame()
    {
        m_game.UpdateGameState(GameState.GAME);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void QuitGame()
    {
        Application.Quit();
    }
    void Options()
    {

    }
    void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
    public void Cancel(InputAction.CallbackContext obj)
    {
        switch (m_game.m_gameState)
        {
            case GameState.OPTIONS:
                if (m_game.m_lastState == GameState.PAUSE)
                {
                    Pause();
                }
                else if (m_game.m_lastState == GameState.MENU)
                {
                    MainMenu();
                }
                break;
            case GameState.PAUSE:
                Resume();
                break;
            case GameState.CINEMATIC:
                break;
            default:
                break;
        }
    }
    void RevertCheckpoint()
    {

    }
    void RevertDefaults()
    {

    }
    void Credits()
    {

    }
    void BonusArt()
    {

    }

    public void SetTutorial(string m_tutorialText)
    {
        m_tutorial.gameObject.SetActive(true);
        m_tutorial.text = m_tutorialText;
    }

    public void ClearTutorial()
    {
        m_tutorial.gameObject.SetActive(false);
    }

    public void SetInteract(RaycastHit hit)
    {
        if (m_interactText.enabled)
            return;
        else
        {
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                m_interactText.enabled = true;
                m_interactText.text = puzzle.m_interactMessage;
            }
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                m_interactText.enabled = true;
                m_interactText.text = interactable.m_interactMessage;
            }
            ManaPool manaPool = hit.transform.gameObject.GetComponent<ManaPool>(); 
            if(manaPool != null)
            {
                m_interactText.enabled = true;
                m_interactText.text = manaPool.m_interactMessage;
            }
        }
    }

    public void StopInteract()
    {
        m_interactText.enabled = false;
    }

    public void UpdateHealth()
    {
        m_health.value = m_player.m_currentHealth;
    }
}
