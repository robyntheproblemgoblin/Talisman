using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class AudioManager : MonoBehaviour
{
    GameManager m_game;
    //Indexes
    int m_initialLinesIndex = 0;
    int m_circlePuzzleIndex = 0;
    int m_rotatingPuzzleIndex = 0;
    int m_finalLinesIndex = 0;
    [HideInInspector] public int m_musicIndex;

    public List<DialogueList> m_dialogues;
    Dictionary<string, List<AudioSubtitle>> m_dictionary;

    public AudioSubtitle m_intro;
    [Header("Voice Lines Alpha Build")]
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

    [Header("Cinematic Sources")]
    public AudioSource[] m_playerSources;
    [HideInInspector]
    public double m_initDelayLines = 0.01d;
    int m_playerSourceToggle;
    double m_nextStartTime;
    bool m_playLines = true;

    [Header("Menu Source")]
    public AudioSource m_menuSource;
    public List<AudioClip> m_menuClips;

    private void Start()
    {
        m_game = GameManager.Instance;
        m_game.m_audioManager = this;
        m_dictionary = new Dictionary<string, List<AudioSubtitle>>();
        foreach(DialogueList dl in m_dialogues)
        {
            m_dictionary.Add(dl.m_section, dl.m_audioList);
        }
    }

    private void Update()
    {

    }
    
    public async UniTask PlayInitialVoiceLines()
    {
        while (m_playLines)
        {
            if (AudioSettings.dspTime > m_nextStartTime - 1)
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
            }
            if (m_initialLinesIndex >= m_voiceLinesInitial.Count)
            {
                m_playLines = false;
            }
            await UniTask.Yield();
        }    
    }

    public async UniTask PlayVoiceSequence(string reference)
    {
        List<AudioSubtitle> current = m_dictionary[reference];
        int index = 0;
        m_playLines = true;
        while (m_playLines)
        {
            if (AudioSettings.dspTime > m_nextStartTime - 1)
            {
                AudioClip clipToPlay = current[index].m_clip;
                // Loads the next Clip to play and schedules when it will start
                m_playerSources[m_playerSourceToggle].clip = clipToPlay;
                m_playerSources[m_playerSourceToggle].PlayScheduled(m_nextStartTime);
                // Checks how long the Clip will last and updates the Next Start Time with a new value
                double duration = (double)clipToPlay.samples / clipToPlay.frequency;
                m_nextStartTime = m_nextStartTime + duration;
                // Switches the toggle to use the other Audio Source next
                m_playerSourceToggle = 1 - m_playerSourceToggle;
                // Increase the clip index number, reset if it runs out of clips
                index++;
            }
            if (index >= current.Count)
            {
                m_playLines = false;
            }
            await UniTask.Yield();
        }
    }

    public async UniTask PlayIntroEffect()
    {
        if (m_menuClips.Count > 0)
        {
            m_menuSource.clip = m_menuClips[0];
            m_menuSource.Play();
        }
        while(m_menuSource.isPlaying)
        {
            await UniTask.Yield();
        }
        OneOffDialogue(m_intro);
    }

    public void OneOffDialogue(AudioSubtitle ASobject)
    {
        m_playerSources[2].clip = ASobject.m_clip;
        m_game.m_menuManager.SetSubtitle(ASobject.m_subtitle);
        m_playerSources[2].Play();
    }

}
