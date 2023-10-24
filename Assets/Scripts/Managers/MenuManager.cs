using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml.Serialization;

public class MenuManager : MonoBehaviour
{
    public event Action<ControllerType> OnControllerChanged;
    public ControllerType m_currentController;
    InputDevice m_lastDevice;

    public ControllerImages m_keyboardImages;
    public ControllerImages m_xBoxImages;
    public ControllerImages m_pSImages;
    public ControllerImages m_nintendoImages;
    public ControllerImages m_genericImages;
    ControllerImages m_currentImages;    
    bool m_tutorialSpriteFirst = false;
    string m_currentMessage = "";
    List<string> m_currentTutorialStrings = new List<String>();
    List<ControlSprites> m_currentTutorialSprites = new List<ControlSprites>();

    #region UI Category Objects
    //public GameObject m_title;
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
    //[Space(5), Header("Title Screen"), Space(5)]
    //public Button m_startButton;
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
    public Button m_controllerImage;
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

        InputSystem.onEvent += InputDeviceChanged;
        OnControllerChanged += SwapControls;
        m_currentImages = m_keyboardImages;

        m_game.OnGameStateChanged += OnGameStateChanged;
        m_falseQuit.onClick.AddListener(delegate () { QuitGame(); });

        //Title Screen Setup
        //m_startButton.onClick.AddListener(delegate () { MainMenu(); });

        //Main Menu Setup
        m_newGame.onClick.AddListener(delegate () { StartGame(); });
        m_menuOptions.onClick.AddListener(delegate () { Options(); });
        m_credits.onClick.AddListener(delegate () { Credits(); });
        m_quit.onClick.AddListener(delegate () { QuitGame(); });

        //HUD Setup
        m_health.maxValue = m_player.m_health;
        m_mana.maxValue = m_player.m_maxMana;

        //Cinematic Setup

        //Pause Menu Setup
        m_resume.onClick.AddListener(delegate () { Resume(); });
        m_pauseOptions.onClick.AddListener(delegate () { Options(); });
        m_controllerImage.onClick.AddListener(delegate () { ShowControllerImages(); });
        m_quitMenu.onClick.AddListener(delegate () { TitleScreen(); });

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

    }

    public void InputDeviceChanged(InputEventPtr eventPtr, InputDevice device)
    {
        if (m_lastDevice == device) return;

        if (eventPtr.type != StateEvent.Type) return;

        bool validPress = false;
        foreach (InputControl control in eventPtr.EnumerateChangedControls(device, 0.01F))
        {
            validPress = true;
            break;
        }
        if (validPress is false) return;

        if (device is Keyboard || device is Mouse)
        {
            if (m_currentController == ControllerType.KEYBOARD) return;
            OnControllerChanged?.Invoke(ControllerType.KEYBOARD);
        }
        if (device is XInputController)
        {
            OnControllerChanged?.Invoke(ControllerType.XBOX);
        }
        else if (device is DualShockGamepad)
        {
            OnControllerChanged?.Invoke(ControllerType.PS);
        }
        else if (device is SwitchProControllerHID)
        {
            OnControllerChanged?.Invoke(ControllerType.NINTENDO);
        }
        else if (device is Gamepad)
        {
            OnControllerChanged?.Invoke(ControllerType.GENERIC);
        }
    }

    void SwapControls(ControllerType controls)
    {
        m_currentController = controls;
        switch (controls)
        {
            case ControllerType.KEYBOARD:
                UpdateUIImages(m_keyboardImages);
                break;
            case ControllerType.XBOX:
                UpdateUIImages(m_xBoxImages);
                break;
            case ControllerType.PS:
                UpdateUIImages(m_pSImages);
                break;
            case ControllerType.NINTENDO:
                UpdateUIImages(m_nintendoImages);
                break;
            case ControllerType.GENERIC:
                UpdateUIImages(m_genericImages);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(controls), controls, null);
        }
    }

    void UpdateUIImages(ControllerImages ci)
    {
        m_currentImages = ci;
        if (m_tutorial.isActiveAndEnabled)
        {
            SetTutorial(m_currentTutorialStrings, m_currentTutorialSprites, m_tutorialSpriteFirst);
        }
        UpdateInteract();
    }

    void UpdateInteract()
    {
        if (m_interactText.enabled)
        {             
            string interact = (SpriteToString(ControlSprites.INTERACT_ONE) + "/" + SpriteToString(ControlSprites.INTERACT_TWO) + " " + m_currentMessage);                        
            m_interactText.text = interact;
        }
    }

    void OnGameStateChanged(GameState state)
    {
        UpdateUI(state);
        switch (state)
        {
            case GameState.TITLE:
                MainMenu();
                break;
            case GameState.MENU:
                MainMenu();
                break;
            case GameState.GAME:
                if (m_game.m_lastState == GameState.PAUSE || m_game.m_lastState == GameState.DEATH)
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void MainMenu()
    {
        m_game.UpdateGameState(GameState.MENU);
        m_eventSystem.SetSelectedGameObject(m_newGame.gameObject);
        Cursor.lockState = CursorLockMode.Confined;
    }
    void StartGame()
    {
        m_game.m_audioManager.PlayIntroDialogue();
        m_game.UpdateGameState(GameState.GAME);
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Resume()
    {
        m_game.UpdateGameState(GameState.GAME);
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
        /*   m_game.UpdateGameState(GameState.OPTIONS);
           m_eventSystem.SetSelectedGameObject(m_camSensitivityButton.gameObject);*/
    }

    void ShowControllerImages()
    {

    }

    void Pause()
    {
        m_eventSystem.SetSelectedGameObject(m_resume.gameObject);
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

    void Credits()
    {

    }

    public void SetTutorial(List<string> text, List<ControlSprites> sprites, bool spriteFirst)
    {
        m_currentTutorialStrings = text;
        m_currentTutorialSprites = sprites;
        m_tutorialSpriteFirst = spriteFirst;
        m_tutorial.gameObject.SetActive(true);
        string message = "";
        int index = 0;
        if (spriteFirst)
        {
            while (index < sprites.Count)
            {
                message += SpriteToString(sprites[index]);
                if (index < text.Count)
                {
                    message += text[index];
                }
                index++;
            }
        }
        else
        {
            while (index < text.Count)
            {
                message += text[index];
                if (index < sprites.Count)
                {
                    message += SpriteToString(sprites[index]);
                }
                index++;
            }
        }
        m_tutorial.text = message;
    }

    string SpriteToString(ControlSprites cs)
    {
        string sprite = "";
        switch (cs)
        {
            case ControlSprites.MENU_NAV:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_menuNavigation + "> ";
                break;
            case ControlSprites.MENU_SELECT:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_menuSelect + "> ";
                break;
            case ControlSprites.MENU_BACK:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_menuBack + "> ";
                break;
            case ControlSprites.MOVEMENT:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_movement + "> ";
                break;
            case ControlSprites.CAMERA:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_camera + "> ";
                break;
            case ControlSprites.JUMP:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_jump + "> ";
                break;
            case ControlSprites.INTERACT_ONE:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_interactOne + "> ";
                break;
            case ControlSprites.INTERACT_TWO:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_interactTwo + "> ";
                break;
            case ControlSprites.ATTACK:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_attack + "> ";
                break;
            case ControlSprites.BLOCK:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_block + "> ";
                break;
            case ControlSprites.HEAL:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_heal + "> ";
                break;
            case ControlSprites.PAUSE:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_pause + "> ";
                break;
            case ControlSprites.SPRINT:
                sprite = "<sprite=\"SS_" + m_currentImages.m_spriteAsset + "\" index= " + m_currentImages.m_sprint + "> ";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cs), cs, null);
        }
        return sprite;
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
            string interact = (SpriteToString(ControlSprites.INTERACT_ONE) + "/" + SpriteToString(ControlSprites.INTERACT_TWO) + " ");
            Puzzle puzzle = hit.transform.gameObject.GetComponentInParent<Puzzle>();
            if (puzzle != null)
            {
                interact += puzzle.m_interactMessage;
                m_currentMessage = puzzle.m_interactMessage;
            }
            Interactable interactable = hit.transform.gameObject.GetComponentInParent<Interactable>();
            if (interactable != null)
            {
                interact += interactable.m_interactMessage;
                m_currentMessage = interactable.m_interactMessage;
            }
            ManaPool manaPool = hit.transform.gameObject.GetComponent<ManaPool>();
            if (manaPool != null)
            {
                interact += manaPool.m_interactMessage;
                m_currentMessage = manaPool.m_interactMessage;
            }
            m_interactText.enabled = true;
            m_interactText.text = interact;
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
        if (!m_subtitles.gameObject.activeSelf)
        {
            m_subtitles.gameObject.SetActive(true);
        }
        m_subtitles.text = subtitile;
        SubtitleTimeOut(Time.time).Forget();
    }

    async UniTask SubtitleTimeOut(float startTime)
    {
        string currentSub = m_subtitles.text;
        while (Time.time <= startTime + m_subtitleTime)
        {
            await UniTask.Yield();
        }
        if (m_subtitles.text == currentSub)
        {
            m_subtitles.text = string.Empty;
        }
    }

    public async UniTask HitReticle()
    {
        float startTime = Time.time;
        m_reticleHit.enabled = true;
        while (Time.time <= startTime + m_reticleHitTime)
        {
            await UniTask.Yield();
        }
        if (m_reticleHit.enabled == true)
        {
            m_reticleHit.enabled = false;
        }
    }


}

public enum ControllerType
{
    KEYBOARD,
    PS,
    XBOX,
    NINTENDO,
    GENERIC
}