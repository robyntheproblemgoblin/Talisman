using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;
    //Indexes
    int m_initialLinesIndex = 0;
    int m_circlePuzzleIndex = 0;
    int m_rotatingPuzzleIndex = 0;
    int m_finalLinesIndex = 0;
    [HideInInspector] public int m_musicIndex;

    [Header("Voice Lines")]
    public List<AudioClip> m_voiceLinesInitial;
    public List<AudioClip> m_voiceLinesPuzzleLeft;
    public List<AudioClip> m_voiceLinesPuzzleRight;
    public List<AudioClip> m_voiceLinesEnd;

    [Header("Music")]
    public List<AudioClip> m_music;
    public AudioSource m_musicSource;

    [Header("SFX")]
    public List<AudioClip> m_soundEffects;
    public AudioSource m_sfxSource;

    //Source on other things? dragged into this? 
    //Source is object in world so put them where you want them in 3D space attached to a GameObject. Drag them into place in the inspector.
    //For this set of the variable it should be an array of 2 AudioSources. I kept them playerSources as a suggestion that as this is dialogue
    //from the player and the items in the players hands the sources can go onto the player. Will develop it more after alpha. Any more is currently
    //vetoed until further notice as it is not important to the alpha loop.
    [Header("Cinematic Sources")]
    public AudioSource[] m_playerSources;
    [HideInInspector]
    public double m_initDelayLines = 0.01d;
    int m_playerSourceToggle;
    double m_nextStartTime;
    bool m_playLines = false;

    [Header("Menu Source")]
    public AudioSource m_menuSource;
    public List<AudioClip> m_menuClips;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;        
    }


    private void Update()
    {
        /*if (*//*AudioSettings.dspTime > m_nextStartTime - 1*//*m_playLines && m_initialLinesIndex <= m_voiceLinesInitial.Count)
        {
            AudioClip clipToPlay = m_voiceLinesInitial[m_initialLinesIndex];
            // Loads the next Clip to play and schedules when it will start
            m_playerSources[m_playerSourceToggle].clip = clipToPlay;
            m_playerSources[m_playerSourceToggle].PlayScheduled(m_nextStartTime);
            // Checks how long the Clip will last and updates the Next Start Time with a new value
            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            m_nextStartTime = m_nextStartTime + duration;
            // Switches the toggle to use the other Audio Source next
            m_playerSourceToggle = 1 - m_playerSourceToggle;
            // Increase the clip index number, reset if it runs out of clips
            m_initialLinesIndex++;
        }*/
    }

    /* Depending on how/when you want this called I see it as to potential options:
     * 1. You can make a small script and attach it to an empty game object with a collider that calls:
     *     void OnTriggerEnter(Collider other)
            {
                if(other.gameObject.GetComponentInParent<PlayerController>() != null) 
                  {        
                    AudioManager.Instance.PlayInitialVoiceLines();
                  }
            }
     * 2. You can tell me when to trigger it if it's off something like the cinematic trigger etc.    
     */
    public void PlayInitialVoiceLines()
    {
        if (m_initialLinesIndex == 0)
        {
            AudioClip clipToPlay = m_voiceLinesInitial[m_initialLinesIndex];
            m_playerSources[m_playerSourceToggle].clip = clipToPlay;
            m_playerSources[m_playerSourceToggle].PlayScheduled(AudioSettings.dspTime + m_initDelayLines);
            m_playerSourceToggle = 1 - m_playerSourceToggle;
            m_initialLinesIndex++;
            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            m_nextStartTime = (m_nextStartTime + duration);
            m_playLines = true;
        }
    }

    public void PlayIntroEffect()
    {
        if (m_menuClips.Count > 0)
        {
            m_menuSource.clip = m_menuClips[0];
            m_menuSource.Play();
        }
    }

    /*

    I kept these for reference later. 
    public void PlayCirclePuzzleClip()
    {
        m_source.clip = m_voiceLinesInitial[m_circlePuzzleIndex];
        m_source.Play();
        m_circlePuzzleIndex++;
    }
    public void PlayRotatingPuzzleClip()
    {
        m_source.clip = m_voiceLinesInitial[m_rotatingPuzzleIndex];
        m_source.Play();
        m_rotatingPuzzleIndex++;
    }
    public void PlayFinalVoiceLines()
    {
        m_source.clip = m_voiceLinesInitial[m_finalLinesIndex];
        m_source.Play();
        m_finalLinesIndex++;
    }
    public void PlayMusic()
    {
        m_source.clip = m_music[m_musicIndex];
        m_source.Play();
    }*/ 

}
