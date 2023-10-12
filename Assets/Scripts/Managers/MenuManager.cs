using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using Cysharp.Threading.Tasks;

public class MenuManager : MonoBehaviour
{
    #region UI Category Objects
    public GameObject m_title;
    public GameObject m_mainMenu;
    public GameObject m_hud;
    public GameObject m_cinematics;
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    public GameObject m_deathScreen;
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
    [Space(5)]
    public float m_subtitleTime = 2;
    bool m_showSubtitle = true;
    public Image m_reticleHit;
    public float m_reticleHitTime = 0.5f;
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

    #region Death Fields
    public Image m_deathImage;
    public Button m_respawnButton;
    public Button m_deathQuit;
    public float m_deathFade = 0.33f;
    #endregion

    //Credits

    GameManager m_game;
    public PlayerController m_player;    
    public EventSystem m_eventSystem;

    private void Start()
    {
        m_game = GameManager.Instance;
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

        //Death Setup
        m_respawnButton.onClick.AddListener(delegate () { Respawn(); });
        m_respawnButton.gameObject.SetActive(false);
        m_deathQuit.onClick.AddListener(delegate () { QuitGame(); });
        m_deathQuit.gameObject.SetActive(false);

        m_health.maxValue = m_player.m_health;
        m_mana.maxValue = m_player.m_maxMana;
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
                else if (m_game.m_lastState == GameState.PAUSE || m_game.m_lastState == GameState.DEATH)
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
        //m_title.SetActive(state == GameState.TITLE);
        m_mainMenu.SetActive(state == GameState.MENU || state == GameState.TITLE);
        m_hud.SetActive(state == GameState.GAME);
        m_cinematics.SetActive(state == GameState.CINEMATIC);
        m_pauseMenu.SetActive(state == GameState.PAUSE);
        m_optionsMenu.SetActive(state == GameState.OPTIONS);
        m_deathScreen.SetActive(state == GameState.DEATH);
        m_subtitles.gameObject.SetActive(state == GameState.CINEMATIC || state == GameState.GAME);
    }
    void TitleScreen()
    {
        m_game.UpdateGameState(GameState.MENU);
        m_eventSystem.SetSelectedGameObject(m_newGame.gameObject);
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
        m_game.m_audioManager.PlayIntroEffect();
        m_game.UpdateGameState(GameState.GAME);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        if (m_health.value <= 0)
        {
            m_game.UpdateGameState(GameState.DEATH);
        }
    }

    public void UpdateMana()
    {
        m_mana.value = m_player.m_currentMana;
    }

    public async UniTask FadeDeathScreen()
    {
        float alpha = 0;
        while (alpha < 1)
        {
            m_deathImage.color = new Color(0, 0, 0, alpha);
            alpha += Time.deltaTime * m_deathFade;
            await UniTask.Yield();
        }
        SetDeathScreen();
    }

    void SetDeathScreen()
    {        
        m_respawnButton.gameObject.SetActive(true);
        m_deathQuit.gameObject.SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_respawnButton.gameObject);
    }


    void Respawn()
    {
        m_respawnButton.gameObject.SetActive(false);
        m_deathQuit.gameObject.SetActive(false); 
        m_deathImage.color = new Color(0, 0, 0, 0);
        m_game.Respawn();
    }

    public async void SetSubtitle(string subtitile)
    {
        if(!m_subtitles.gameObject.activeSelf)
        {
            m_subtitles.gameObject.SetActive(true);
        }
        m_subtitles.text = subtitile;
        SubtitleTimeOut(Time.time).Forget();
    }    

    async UniTask SubtitleTimeOut(float startTime)
    {
        string currentSub = m_subtitles.text;
        while(Time.time <= startTime + m_subtitleTime)
        {
          await UniTask.Yield();
        }
        if(m_subtitles.text == currentSub)
        {
            m_subtitles.text = string.Empty;            
        }
    }

    public async UniTask HitReticle()
    {
        float startTime = Time.time;
        m_reticleHit.enabled = true;
        while(Time.time <= startTime + m_reticleHitTime)
        {
            await UniTask.Yield();
        }
        if(m_reticleHit.enabled == true)
        {
            m_reticleHit.enabled = false;
        }
    }
}
