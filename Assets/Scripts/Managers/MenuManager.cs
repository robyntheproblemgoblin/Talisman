using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    #region UI Category Objects
    GameObject m_title;
    GameObject m_mainMenu;
    GameObject m_pauseMenu;
    GameObject m_optionsMenu;
    GameObject m_hud;
    #endregion

    #region Title Screen Fields
    //[Space(5), Header("Title Screen"), Space(5)]
    Button m_startButton;
    Camera m_titleCamera;
    #endregion

    #region Main Menu Fields
    //[Header("Main Menu"), Space(5)]
    Button m_newGame;
    Button m_menuOptions;
    Button m_credits;
    Button m_bonusArt;
    Button m_quit;
    #endregion

    #region Pause Menu Fields
    //[Header("Pause Menu"), Space(5)]
    Button m_resume;
    Button m_pauseOptions;
    Button m_revert;
    Button m_quitMenu;
    #endregion

    #region Options 
    //[Header("Options"), Space(5)]
    Button m_camSensitivityButton;
    Slider m_camSensitivitySlider;
    Button m_subtitlesButton;
    Slider m_subtitlesSlider;
    Button m_vibrationButton;
    Slider m_vibrationSlider;
    Button m_masterVolumeButton;
    Slider m_masterVolumeSlider;
    Button m_musicVolumeButton;
    Slider m_musicVolumeSlider;
    Button m_sfxButton;
    Slider m_sfxSlider;
    Button m_dialogueButton;
    Slider m_dialogueSlider;
    Button m_defaults;
    Button m_optionsBack;
    #endregion
        
    void Awake()
    {
        // Get the Categories
        m_title = GameObject.Find("Title Screen");
        m_mainMenu = GameObject.Find("Main Menu");
        m_pauseMenu = GameObject.Find("Pause Menu");
        m_optionsMenu = GameObject.Find("Options");
        m_hud = GameObject.Find("HUD");

        //Get Title Screen Fields
        m_startButton = GameObject.Find("Start Button").GetComponent<Button>();
        m_titleCamera = GameObject.Find("Title Camera").GetComponent<Camera>();

        //Get Main Menu Fields
        m_newGame = GameObject.Find("New Game Button").GetComponent<Button>();
        m_menuOptions= GameObject.Find("Menu Options Button").GetComponent<Button>();
        m_credits = GameObject.Find("Credits Button").GetComponent<Button>();
        m_bonusArt = GameObject.Find("Bonus Art Button").GetComponent<Button>();
        m_quit = GameObject.Find("Quit Button").GetComponent<Button>();     

        //Get Pause Fields
        m_resume = GameObject.Find("Resume Button").GetComponent<Button>();
        m_pauseOptions = GameObject.Find("Options Button").GetComponent<Button>();
        m_revert = GameObject.Find("Revert Button").GetComponent<Button>();
        m_quitMenu = GameObject.Find("Pause Quit Button").GetComponent<Button>();

        //Get Options Fields
        m_camSensitivityButton = GameObject.Find("Camera Sensitivity").GetComponent<Button>();
        m_camSensitivitySlider = GameObject.Find("Camera Sensitivity Slider").GetComponent<Slider>();
        m_subtitlesButton = GameObject.Find("Subtitles").GetComponent<Button>();
        m_subtitlesSlider = GameObject.Find("Subtitles Slider").GetComponent<Slider>();
        m_vibrationButton = GameObject.Find("Controller Vibration").GetComponent<Button>();
        m_vibrationSlider = GameObject.Find("Controller Vibration Slider").GetComponent<Slider>();
        m_masterVolumeButton = GameObject.Find("Master Volume").GetComponent<Button>();
        m_masterVolumeSlider = GameObject.Find("Master Volume Slider").GetComponent<Slider>();
        m_musicVolumeButton = GameObject.Find("Music").GetComponent<Button>();
        m_musicVolumeSlider = GameObject.Find("Music Volume Slider").GetComponent<Slider>();
        m_sfxButton = GameObject.Find("Sound Effects").GetComponent<Button>();
        m_sfxSlider = GameObject.Find("Sound Effects Volume Slider").GetComponent<Slider>();
        m_dialogueButton = GameObject.Find("Dialogue").GetComponent<Button>();
        m_dialogueSlider = GameObject.Find("Dialogue Volume Slider").GetComponent<Slider>();
        m_defaults = GameObject.Find("Reset Defaults").GetComponent<Button>();
        m_optionsBack = GameObject.Find("Options Back Button").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
